using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class CarsTripsDAL : BaseDAL, ICarsTripsDAL
    {
        public CarsTripsDAL(IConfiguration configuration) : base(configuration)
        {
        }


        public CarsTripsDAL() : base()
        {
        }

        public async Task<bool> InsertCarTripAsync(CarsTrips carsTrip)
        {

            string query = $@"INSERT INTO [dbo].[CarsTrips] ([CreationDate] ,[CarsQueueId] ,[DriverId], [CarId]) 
                                    VALUES (@CreationDate, @CarsQueueId, @UserId, @CarId )";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { carsTrip.CarsQueueId, UserId = carsTrip.DriverId, carsTrip.CreationDate, carsTrip.CarId }) > 0;
            }
        }

        public async Task<bool> IsCarTripExistAsync(int carqId)
        {
            string query = $@"select id from CarsTrips
                            where CarsQueueID = @carqId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carqId });

                return result.FirstOrDefault() > 0;
            }
        }

        public async Task<int> GetCarTripByCarsQueueIDAsync(int carqId)
        {
            string query = $@"select id from CarsTrips
                            where CarsQueueID = @carqId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carqId });

                return result.FirstOrDefault();
            }
        }

        public IList<CarsTrips> GetAll()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = "SELECT * FROM CarsTrips ct, CarsQueue cq, Users u, Cars c" +
                    "           WHERE ct.CarsQueueId = cq.Id and ct.DriverId = u.Id AND cq.CarId = c.Id" +
                    "           ORDER BY ct.CreationDate DESC";
                return connection.Query<CarsTrips, CarsQueue, User, Car, CarsTrips>(sql,
                    (carTrip, carQueue, user, car) =>
                    {
                        carQueue.Car = car;
                        carTrip.CarsQueue = carQueue;
                        carTrip.User = user;

                        return carTrip;
                    },
                    splitOn: "Id, Id"
                    ).ToList();

            }
        }

        public async Task<Driver> GetDriverByCarQIdAsync(int id)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"SELECT d.*, u.* 
                               FROM CarsTrips ct, Drivers d, Users u WHERE u.id = d.userId and  ct.CarsQueueId = @Id  AND ct.DriverId = d.UserId";
                var result = await connection.QueryAsync<Driver, User, Driver>(query, (d, u) =>
                  {
                      d.User = u;
                      return d;
                  },
                new { Id = id },
                splitOn: "id"
                );
                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetRiderQIdFromLastCarTripAsync(string carId, int riderId)
        {

            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"
                                WITH carTrip AS (
                                SELECT top 1 ct.Id FROM CarsTrips ct, CarsQueue cq, Cars c
                                where ct.CarsQueueId = cq.Id
                                and cq.CarId = c.Id
                                and c.CarCode = @carId
                                order by cq.Id desc
                                )
                                SELECT tr.riderQId FROM carTrip ct, TripsRiders tr
                                where ct.Id = tr.TripId
                                and RiderId = @riderId
                                ";

                return (await connection.QueryAsync<int>(query, new { carId, riderId })).FirstOrDefault();
            }
        }

        public async Task<List<RidersTickets>> GetLastCarTripInfoAsync(int carId, string lang)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@" With LastTripRiderQ as (
                               SELECT  rq.Id, rq.CountOfSeats, rq.ReservationDate, rq.RidersQStatusLookupId, 
                                       rq.PickupStationId, rq.RiderId, rq.IsPresent
                                        FROM TripsRiders Tr, CarsTrips ct, RidersQueue rq
                                        Where 
                                        Tr.TripId = ct.Id
                                        and  tr.riderQId = rq.Id
                                        and ct.CarId = @carId
                                        and ct.Id in (
                                        select Max(Id) FROM CarsTrips
                                        where CarId = @carId
                                        )
                                )
                                SELECT rq.*, rqLookup.Id, rqLookup.Name, rtik.Id, rtik.Ticket, r.Id, r.Name, psTran.PickupStationId as 'Id', psTran.Name
                                FROM  RidersQStatusLookup rqLookup, Users r,  PickupStationsTranslations psTran, LastTripRiderQ rq left join RidersTickets rtik on rq.Id = rtik.RiderQId
                                WHERE  rq.RidersQStatusLookupId = rqLookup.Id
                                and r.Id = rq.RiderId
                                and psTran.PickupStationId= rq.PickupStationId
                                and psTran.Lang = @lang
                                ";

                return (await connection.QueryAsync<RidersQueue, RidersQStatusLookup, RidersTickets, User, PickupStations,  RidersTickets>(query,
                       (rq, rqlook, rtick, u, ps) =>
                       {
                           rq.RidersQStatusLookup = rqlook;
                           rq.User = u;
                           rq.PickupStations = ps;
                           if(rtick == null)
                           {
                               rtick = new RidersTickets() { 
                               Ticket = "",
                               };
                           }

                           rtick.RidersQueue = rq;
                           return rtick;
                       },
                       new { carId, lang },
                       splitOn: "Id,Id,Id,Id"
                       )).ToList();
            }
        }
    }
}
