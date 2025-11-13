using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class CarsQueueDAL : BaseDAL, ICarsQueueDAL
    {
        public CarsQueueDAL(IConfiguration configuration) : base(configuration)
        {
        }

        private const string CarsQueueTable = "CarsQueue";
        public CarsQueueDAL() : base()
        {
        }

        public async Task<int> AddCarToTheQueueAsync(CarsQueue carsQueue)
        {
            string query = $@"
                                declare @maxTurn int = (Select max(Turn) from CarsQueue
                                WHERE  PickupStationId =  @PickupStationId
                                and CarsQStatusLookupId =  {(int)CarsQStatusLookupEnum.InQueue})

                                declare @rank int = (SELECT ISNULL(@maxTurn + 1, 1 ))

                              INSERT into CarsQueue (CarId, CreationDate, PickupStationId,CarsQStatusLookupId, SkipCount, turn) 
                              Values (@CarId, @CreationDate, @PickupStationId,@CarsQStatusLookupId, @SkipCount, @rank )
                              SELECT SCOPE_IDENTITY() 
                              ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carsQueue.CarId, carsQueue.PickupStationId, carsQueue.CreationDate, carsQueue.CarsQStatusLookupId, carsQueue.SkipCount, carsQueue.Skip/*, carsQueue.Turn*/ });

                return result.FirstOrDefault();
            }
        }
        public async Task<bool> IsCarInQueueAsync(int carId)
        {
            string query = $@"  select Id from CarsQueue
                                where 
                                CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue} 
                                and CarId = @carId
                                order by id desc
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int>(query, new
                {
                    carId
                    /*, CreationDate = DateTime.Now*/ /*DateTime.Now.Date.ToString("MM/dd/yyyy") */
                }
                );

                return res.FirstOrDefault() > 0;
            }
        }

        //public int GetCountOfCarsInPickupStation(int psId)
        //{
        //    string query = $@"select count(id) from CarsQueue
        //                        where convert(date, CreationDate) = @creationDate
        //                        and CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue} 
        //                        and PickupStationId = @psId
        //                        ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Query<int>(query, new { psId, CreationDate = DateTime.Now.Date.ToString("MM/dd/yyyy") }).FirstOrDefault();
        //    }
        //}
        public async Task<bool> IsCarTurnActiveInTheStationAsync(int carId, int psId)
        {
            string query = $@"select Id from CarsQueue
                                where 
                                 CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                 and PickupStationId = @psId
                                 and CarId = @carId
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int>(query, new { carId, psId/*, CreationDate = DateTime.Now.Date.ToString("MM/dd/yyyy")*/ });

                return res.FirstOrDefault() > 0;
            }
        }
        public async Task<bool> UpdateCarSkipCountAsync(int carId, int skipCount)
        {
            string query = $@"WITH UpdateCarsQueue AS (
                                SELECT TOP 1  * from CarsQueue 
	                            where CarId = @carId
                                and CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue} 
                                order by creationDate asc
                            )
                            update UpdateCarsQueue 
                            set SkipCount += @skipCount "; /*, turn += @SkipCount*/

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { carId, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"), skipCount }) > 0;
            }
        }

        // Bug in Query CarsQueue should be UpdateCarsQueue
        public async Task<bool> UpdateCarStatusInQueueAsync(int carId, int psId, CarsQStatusLookupEnum status)
        {
            string query = $@"WITH UpdateCarsQueue AS (
                                SELECT TOP 1  * from CarsQueue 
	                            where CarId = @carId
                                and PickupStationId = @psId
                                and CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                order by id desc
                            )
                            update UpdateCarsQueue
                            set CarsQStatusLookupId = @status
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { carId, psId/*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/, status = (int)status }) > 0;
            }
        }

        public async Task<CarsQueue> GetActiveCarInCarQAsync(int carId)
        {
            string query = $@"
                            SELECT TOP 1  cq.Id, cq.CreationDate, cq.CarId, cq.SkipCount, cq.PickupStationId, 
                            cq.CarsQStatusLookupId,  cq.IsNotifiedAboutTurn, cq.turn,
                            c.Id, c.ItineraryId, c.LoggedInDriverId, c.SeatCount
                            FROM CarsQueue cq, Cars c
                            WHERE cq.CarId = c.Id and
                            cq.CarId = @carId
                            and cq.CarsQStatusLookupId = { (int)CarsQStatusLookupEnum.InQueue}
                            order by cq.CreationDate asc
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<CarsQueue, Car, CarsQueue>(query, (cq, c) =>
                {
                    cq.Car = c;
                    return cq;
                }, new { carId/*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/ },
                splitOn: "Id"
                );

                return result.FirstOrDefault();
            }
        }

        //public CarsQueue GetCarTurnInPickup(int pickupId)
        //{

        //    string query = $@"  SELECT cq.Id, cq.CreationDate, cq.CarId, cq.SkipCount, cq.PickupStationId, 
        //                                  cq.CarsQStatusLookupId, cq.Skip, cq.IsNotifiedAboutTurn, c.Id, c.ItineraryId, c.LoggedInDriverId,
        //                                  c.SeatCount from CarsQueue cq, Cars c
        //                        where cq.CarId = c.Id and
        //                        PickupStationId = @pickupId
        //                        and CarsQStatusLookupId =  {(int)CarsQStatusLookupEnum.InQueue} 
        //                        and  skip = skipCount
        //                        order by cq.CreationDate asc
        //                        ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Query<CarsQueue,
        //            Car,
        //            CarsQueue>(query, (cq, c) =>
        //            {
        //                cq.Car = c;
        //                return cq;
        //            }, new { pickupId/*, creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd") */},
        //            splitOn: "Id"
        //            ).FirstOrDefault();
        //    }
        //}
        // Need to be changed
        public async Task<Driver> GetActiveDriverInPickupQAsync(int pickupId)
        {
            string query = $@"Select carq.Id, carq.CreationDate, carq.CarId, carq.SkipCount, carq.PickupStationId,
                                carq.CarsQStatusLookupId, 
                                car.Id, car.carCode, car.LoggedInDriverId, car.RegistrationPlate,
                                d.UserId, d.LicenseNumber,
                                u.Id, u.Name, u.FCMToken, u.Language
                              FROM CarsQueue carq, Cars car, Drivers d, Users u
                              where PickupStationId = @pickupId
                                and carq.CarId = car.Id and car.LoggedInDriverId = d.UserId
                                and d.UserId = u.Id
                                and carq.CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                and carq.Turn = 1
                              order by carq.creationDate asc";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<Driver, Car, User, Driver>(query, (d, c, u) =>
                {
                    d.User = u;
                    d.Car = c;
                    return d;
                },
                new { pickupId /*creationDate = DateTime.Now.Date.ToString("yyyy-MM-dd")*/ });

                return result.FirstOrDefault();
            }
        }

        public async Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId)
        {
            string query = $@"  Select carq.Id, carq.CreationDate, carq.CarId, carq.SkipCount, carq.PickupStationId,
                                carq.CarsQStatusLookupId, carq.SkipCount, carq.isNotifiedAboutTurn,
                                car.Id, car.LoggedInDriverId, car.RegistrationPlate, car.CarCode,
                                d.UserId, d.LicenseNumber,
                                u.Id, u.Name, u.FCMToken
                                FROM CarsQueue carq, Cars car, Drivers d, Users u
                                where PickupStationId = @pickupId
                                and carq.CarId= car.Id and car.LoggedInDriverId = d.UserId
                                and d.UserId = u.Id
                                and carq.Turn = 1
                                and carq.CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                order by carq.creationDate asc
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<CarsQueue, Car, Driver, User, CarsQueue>(query, (carq, car, driver, user) =>
               {
                   driver.User = user;
                   car.LoggedInDriver = driver;
                   carq.Car = car;
                   return carq;
               },
                new { pickupId },
                splitOn: "Id, UserId,Id"
                );
                return result.FirstOrDefault();
            }
        }

        public async Task<CarsQueue> GetCurrentActiveCarsTurnInPickupStationWithSkippedAsync(int pickupId)
        {
            string query = $@"  Select carq.Id, carq.CreationDate, carq.CarId, carq.SkipCount, carq.PickupStationId,
                                carq.CarsQStatusLookupId, carq.Skip, carq.isNotifiedAboutTurn,
                                car.Id, car.LoggedInDriverId, car.RegistrationPlate,
                                d.UserId, d.LicenseNumber,
                                u.Id, u.Name, u.FCMToken
                                FROM CarsQueue carq, Cars car, Drivers d, Users u
                                where PickupStationId = @pickupId
                                and carq.CarId= car.Id and car.LoggedInDriverId = d.UserId
                                and d.UserId = u.Id
                                and carq.CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                and carq.Skip = carq.SkipCount
                                order by carq.creationDate asc
                                ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<CarsQueue, Car, Driver, User, CarsQueue>(query, (carq, car, driver, user) =>
                {
                    driver.User = user;
                    car.LoggedInDriver = driver;
                    carq.Car = car;
                    return carq;
                },
                new { pickupId },
                splitOn: "Id, UserId,Id"
                );

                return result.FirstOrDefault();
            }
        }

        public async Task<List<CarsQueue>> GetDriversInPickupQAsync(int pickupId, CarsQStatusLookupEnum status)
        {
            string query = $@"Select carq.Id, carq.Turn, carq.CreationDate, carq.CarId, carq.SkipCount, carq.PickupStationId,
                              carq.CarsQStatusLookupId, carq.SkipCount,  car.Id, car.LoggedInDriverId, car.RegistrationPlate, car.CarCode,
                              d.UserId, d.LicenseNumber,
                              u.Id, u.Name, u.FCMToken
                              FROM CarsQueue carq, Cars car, Drivers d, Users u
                              where PickupStationId = @pickupId
                              and carq.CarId= car.Id and car.LoggedInDriverId = d.UserId
                              and d.UserId = u.Id
                              and carq.CarsQStatusLookupId = {(int)status}
                              order by Turn 
                           ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<CarsQueue, Car, Driver, User, CarsQueue>(query, (cq, c, d, u) =>
              {
                  cq.Car = c;
                  cq.Car.LoggedInDriver = d;
                  cq.Car.LoggedInDriver.User = u;
                  return cq;
              },
               new { pickupId },
               splitOn: "Id, UserId, Id");

                return result.ToList();
            }
        }

        public bool UpdateIsCarNotifiedAboutTurnInPickupStation(int carqId, bool isNoti)
        {
            string query = $@"
                            Update CarsQueue
                            SET IsNotifiedAboutTurn = @isNoti
                            WHERE Id = @carqId
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new { carqId, isNoti }) > 0;
            }
        }

        public async Task<int> GetCountOfCarsBeforeMyTurnAsync(int carId, int psId)
        {
            //string query = $@"  select count(id) from CarsQueue
            //                      where id > (
            //                      select Id
            //                      from CarsQueue
            //                      where carId = @carId 
            //                      and PickupStationId = @psId
            //                      AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
            //                      )
            //                      AND PickupStationId = @psId
            //                      AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}";

            string query = $@"select count(id) from CarsQueue
                              where 
                              PickupStationId = @psId
                              AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                              and CreationDate < (
                              SELECT CreationDate FROM CarsQueue
                                  WHERE CarId = @carId
                                  AND CarsQStatusLookupId = 1
                              )
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carId, psId });

                return result.FirstOrDefault();
            }
        }


        public async Task<int> GetCountOfCarsAfterMyTurnAsync(int carId, int psId)
        {
            string query = $@"select count(id) from CarsQueue
                                where 
                                PickupStationId = @psId
                                AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                and turn > (
                                SELECT Turn FROM CarsQueue
                                    WHERE CarId = @carId
                                    AND CarsQStatusLookupId = 1
	                                and PickupStationId = @psId
                                )
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carId, psId });

                return result.FirstOrDefault();
            }
        }

        //public int GetCountOfCarsInFrontOfMyTurn(int carId)
        //{
        //    string query = $@"select count(id) from CarsQueue
        //                        where id > (
        //                        select Id
        //                        from CarsQueue
        //                        where carId = @carId 
        //                        AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
        //                        )
        //                        AND  CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Query<int>(query, new { carId }).FirstOrDefault();
        //    }
        //}

        public IList<CarsQueue> GetAllForTodayByItineraryId(int itineraryId, string lang)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"SELECT * 
                                FROM 
                                    {CarsQueueTable} cq, 
                                    Cars c, 
                                    PickupStations ps, 
                                    CarsQStatusLookup cql,  
                                    Transportation_Itineraries ti,
                                    PickupStationsTranslations pts
                                WHERE 
                                    cq.CarId = c.Id AND 
                                    cq.PickupStationId = ps.Id AND 
                                    cq.CarsQStatusLookupId = cql.Id AND 
                                    ps.TransItineraryId = ti.Id AND 
                                    pts.PickupStationId = ps.Id AND Lang LIKE '{lang}'
                                    {(itineraryId == 0 ? "" : $" AND ps.TransItineraryId = {itineraryId}")}  
                                    AND (CAST(cq.CreationDate as date) = CAST(GETDATE() as date) OR CarsQStatusLookupId = 1)
                                ORDER BY cq.CreationDate DESC";

                return connection.Query<CarsQueue, Car, PickupStations, CarsQStatusLookup, Transportation_Itineraries, PickupStationsTranslations, CarsQueue>(sql,
                    (carQueue, car, pickupStations, carQueueLookup, itinery, pickupTranslation) =>
                    {
                        carQueue.Car = car;
                        carQueue.CarsQStatusLookup = carQueueLookup;
                        carQueue.pickupStationsTranslations = pickupTranslation;

                        pickupStations.Transportation_Itineraries = itinery;
                        carQueue.PickupStation = pickupStations;

                        return carQueue;
                    },
                    splitOn: "Id, Id, Id, Id"
                    ).ToList();
            }
        }

        public bool Update(CarsQueue carsQueue)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"UPDATE CarsQueue 
                                SET CarId = @CarId, SkipCount = @SkipCount,  
                                    CarsQStatusLookupId = @CarsQStatusLookupId, 
                                    Turn = @Turn
                                WHERE Id = @Id";
                return connection.Execute(sql,
                    new
                    {
                        carsQueue.Id,
                        carsQueue.CarId,
                        carsQueue.SkipCount,
                        carsQueue.PickupStationId,
                        carsQueue.CarsQStatusLookupId,
                        carsQueue.Turn,
                        carsQueue.IsNotifiedAboutTurn

                    }) > 0;
            }
        }

        public async Task<int> GetCountOfCarsInQueueAsync(int psId)
        {
            string query = $@"SELECT count(id) from CarsQueue
                              WHERE 
                              PickupStationId = @psId
                              AND CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { psId });

                return result.FirstOrDefault();
            }
        }

        //public bool CancelDriverCarReservation(int carId, int psId)
        //{
        //    string query = $@"  update CarsQueue set CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.Cancelled}
        //                        where CarId = @carId 
        //                              and PickupStationId = @psId
        //                    ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Execute(query, new { carId, psId }) > 0;
        //    }
        //}

        //public bool UpdateCarsSkipCount(int carqId, int psId)
        //{
        //    string query = $@" 
        //                        update CarsQueue set Skip += 1
        //                        where id < @carqId
        //                              and PickupStationId = @psId
        //                              and skip < SkipCount
        //                    ";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        return sqlConnection.Execute(query, new
        //        {
        //            carqId,
        //            psId
        //        }) > 0;
        //    }
        //}

        public async Task<CarsQueue> GetCarQueueByTurnAsync(int pickupId, int turn)
        {
            string query = $@"  WITH CarsQueueView as (
                                SELECT *
                                FROM CarsQueue
                                where PickupStationId = @pickupId
                                and Turn =  @turn
                                and CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                                )
                                SELECT cq.Id, cq.CreationDate, cq.CarId, cq.SkipCount, cq.PickupStationId, cq.CarsQStatusLookupId, cq.IsNotifiedAboutTurn, cq.Turn,
	                                   c.Id, c.ItineraryId, c.SeatCount, c.CarCode,
                                       d.UserId, d.CarId,
	                                   us.Id, us.Name, us.FCMToken, us.language
                                FROM  CarsQueueView cq, Cars c, Users us, Drivers d
                                WHERE cq.CarId = c.Id and c.LoggedInDriverId = us.Id and c.LoggedInDriverId = d.UserId
                                and us.Id = d.UserId
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<CarsQueue, Car, Driver, User, CarsQueue>(query, (cq, c, d, us) =>
               {
                   d.User = us;
                   c.LoggedInDriver = d;
                   cq.Car = c;
                   return cq;
               },
                 new { pickupId, turn },
                 splitOn: "Id,Id,UserId,Id"
                 );

                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetQLastCarTurnAsync(int pickupId)
        {
            string query = $@"SELECT max(Turn) from CarsQueue
                              WHERE  PickupStationId = @pickupId
                              and CarsQStatusLookupId = {(int)CarsQStatusLookupEnum.InQueue}
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int?>(query, new { pickupId });

                return result.FirstOrDefault() ?? 0;
            }
        }

        public async Task<int> GetCarQTurnAsync(int pickupId, int carId)
        {

            string query = $@"SELECT Turn from CarsQueue
                              WHERE  PickupStationId = @pickupId
                                     and carId = @carId
                                     and CarsQStatusLookupId =
                                     {(int)CarsQStatusLookupEnum.InQueue}
                             ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var res = await sqlConnection.QueryAsync<int?>(query, new { pickupId, carId });

                return res.FirstOrDefault() ?? 0;
            }
        }

        public bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int fromTurn, int toTurn)
        {
            string query = $@" 
                               update CarsQueue set Turn-=1
                               where PickupStationId = @pickupId
                               and Turn >= @fromTurn
                               and Turn < @toTurn
                               and CarsQStatusLookupId  = {(int)CarsQStatusLookupEnum.InQueue}
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new
                {
                    pickupId,
                    carId,
                    fromTurn,
                    toTurn
                }) > 0;
            }
        }

        public bool UpdateCarsTurnAfterCancelation(int pickupId, int turn)
        {
            string query = $@" 
                               update CarsQueue set Turn-=1
                               where PickupStationId = @pickupId
                               and Turn >= turn
                               and CarsQStatusLookupId  = {(int)CarsQStatusLookupEnum.InQueue}
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Execute(query, new
                {
                    pickupId,
                    turn,
                }) > 0;
            }
        }

        public async Task<DateTime?> GetTheLastTimeCarLeftTheQ(int carId, int psId)
        {
            string query = $@"select LeftingQTime from CarsQueue 
                                    where CarsQStatusLookupId != {(int)CarsQStatusLookupEnum.InQueue} 
                                    and CarId = @carId and PickupStationId = @psId
                                    order by id desc
                            ";

            using (var cn = GetTickAndDashConnection())
            {
                var res = await cn.QueryAsync<DateTime?>(query, new { carId, psId });
                return res.FirstOrDefault();
            }
        }
    }

}
