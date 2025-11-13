using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class RefillRequestsDAL : BaseDAL, IRefillRequestsDAL
    {
        public RefillRequestsDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<RefillRequest>> GetAllRefillRequestsAsync(int driverId)
        {
            string query = $@"SELECT refReq.Id, refReq.RiderId, refReq.Amount,  r.UserId, r.MobileNumber FROM RefillRequests refReq, Riders r
                                    where 
                                    refReq.RiderId= r.UserId and
                                    refReq.DriverId = @driverId 
                                    and DateDiff(SECOND, refReq.CreatedAt, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') < 60 * 60.0
                                    and refReq.IsHandled = 0
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<RefillRequest, Riders, RefillRequest>(query,
                    (refReq, r) =>
                    {
                        refReq.Rider = r;
                        return refReq;
                    },
                    new { driverId },
                    splitOn: "UserId"
                    )).ToList();
            }
        }

        public async Task<int> GetCountOfDriverRefillRequestsAsync(int driverId)
        {
            string query = $@"SELECT count(id) FROM RefillRequests
                                    where  DriverId = @driverId 
                                    and DateDiff(SECOND, CreatedAt, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') < 60 * 60.0
                                    and IsHandled = 0
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { driverId });
            }
        }

        public async Task<RefillRequest> GetRefillRequest(int riderId, int driverId, decimal amount)
        {
            string query = $@"SELECT top 1 * FROM RefillRequests
                                where RiderId = @riderId and DriverId = @driverId 
                                    and amount = @amount 
                                and DateDiff(SECOND, CreatedAt, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') < 60 * 60.0
                                and IsHandled = 0
                                order by CreatedAt desc
                                ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<RefillRequest>(query, new { riderId, driverId, amount });
            }
        }

        public async Task<bool> InsertRefillRequest(RefillRequest refill)
        {
            string query = $@"insert into RefillRequests(RiderId, DriverId, Amount, CreatedAt, IsHandled, IsSuccess) 
                              Values(@RiderId, @DriverId, @Amount, @CreatedAt, @IsHandled, @IsSuccess)
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { refill.RiderId, refill.DriverId, refill.Amount, refill.CreatedAt, refill.IsHandled, refill.IsSuccess }) > 0;
            }
        }

        public async Task<bool> IsThereAnyRequestsToTransferInTheLastNminutesAsync(int riderId, int driverId)
        {
            string query = $@"SELECT id FROM RefillRequests
                                where RiderId = @riderId and DriverId = @driverId 
                                and DateDiff(SECOND, CreatedAt, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}') < 1 * 60.0
                                and IsHandled = 0
                                order by CreatedAt desc
                            ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { riderId, driverId }) > 0;
            }
        }

        public async Task<bool> UpdateRefillRequest(RefillRequest refill)
        {

            string query = $@"with cte as (
                                SELECT top 1 * from RefillRequests
                                where RiderId = @RiderId and DriverId = @DriverId 
                                and Amount = @Amount and IsHandled = 0 
                                and DateDiff(SECOND, CreatedAt, '{DateTime.Now.ToString("yyyy - MM - dd HH: mm: ss")}') < 60 * 60.0 
                                order by CreatedAt desc
                                )
                                Update cte
                                SET IsHandled = @IsHandled, IsSuccess = @IsSuccess
                                 ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new
                {
                    refill.RiderId,
                    refill.DriverId,
                    refill.Amount,
                    refill.CreatedAt,
                    refill.IsHandled,
                    refill.IsSuccess
                }) > 0;
            }
        }
    }
}
