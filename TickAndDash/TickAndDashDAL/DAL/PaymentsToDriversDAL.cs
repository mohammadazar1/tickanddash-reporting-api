using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using Dapper;
using System.Linq;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class PaymentsToDriversDAL : BaseDAL, IPaymentsToDriversDAL
    {
        public PaymentsToDriversDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private const string PaymentsToDriversTable = "PaymentsToDrivers";

        public async Task<int> CreateAsync(PaymentToDriver paymentToDriver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO {PaymentsToDriversTable} (DriverId, PaymentAmount, PaymentDate)
                                OUTPUT Inserted.Id    
                                VALUES(@DriverId, @PaymentAmount, @PaymentDate)";

                return await connection.ExecuteScalarAsync<int>(query, paymentToDriver);
            }
        }

        public async Task<IList<PaymentToDriver>> GetAllByFilterAsync(DateTime from, DateTime to, int driverId)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"SELECT * 
	                            FROM PaymentsToDrivers ptd 
		                            LEFT JOIN Drivers d 
			                            ON ptd.DriverId = d.UserId
		                            LEFT JOIN Users u
			                            ON d.UserId = u.Id
	                            WHERE 
                                    CAST(ptd.PaymentDate as Date) >= CAST(@From as Date) AND
                                    CAST(ptd.PaymentDate as Date) <= CAST(@To as Date)
                                    {(driverId == 0 ? "" : "AND d.UserId = @DriverId")}";

                var result = await connection
                    .QueryAsync<PaymentToDriver, Driver, User, PaymentToDriver>(
                    query, 
                    (payment, driver, user) => 
                    {
                        user.Token = "";
                        user.FCMToken = "";
                        user.Language = "";
                        user.RoleId = 0;
                        driver.User = user;
                        payment.Driver = driver;
                        return payment;
                    }, 
                    splitOn: "userId, Id", 
                    param: new { from, to, driverId});

                return result.ToList();

            }
        }
    }
}
