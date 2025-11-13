using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class TripsRidersDAL : BaseDAL, ITripsRidersDAL
    {
        public TripsRidersDAL(IConfiguration configuration) : base(configuration)
        {
        }


        public TripsRidersDAL()
        {

        }

        public async Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId)
        {
            string query = $@"insert into TripsRiders(RiderId, riderQId, TripId, CreationDate) 
                              Values(@riderId, @riderQId, @tripId, @CreationDate)
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { riderQId, riderId, tripId, CreationDate = System.DateTime.Now }) > 0;
            }
        }

        public class GetRiderTripsAndCancellationCountResponse
        {
            public List<GetRiderTripsResponse> Trips { get; set; } = new List<GetRiderTripsResponse>();
            public int CancellationCount { get; set; }
        }

        public class GetRiderTripsResponse
        {
            public int Id { get; set; }
            public string DriverName { get; set; }
            public DateTime CreationDate { get; set; }
            public string Itinerary { get; set; }
            public string CarRegistrationPlate { get; set; }
            public string CarModel { get; set; }
        }

        //public List<TripsRiders> GetRidersTrip(int riderId)
        public async Task<List<GetRiderTripsResponse>> GetRidersTripAsync(int riderId, string language)
        {
            //string query = $@"
            //                select tr.Id, tr.CreationDate, 
            //                    ct.Id, 
            //                    d.UserId,
            //                    c.Model,
            //                    c.RegistrationPlate,
            //                    u.Id, u.Name,
            //                    carq.Id,  
            //                    ps.Id,
            //                    itir.Id, itir.Name 
            //                from TripsRiders tr, CarsTrips ct, 
            //                Drivers d, Users u, CarsQueue carq, PickupStations ps, Transportation_Itineraries itir, Cars c
            //                where tr.TripId = ct.Id
            //                and ct.DriverId = d.UserId
            //                and u.Id = d.UserId
            //                and carq.Id = ct.CarsQueueId
            //                and ps.Id = carq.PickupStationId
            //                and itir.Id = ps.TransItineraryId
            //                and d.CarId = c.Id
            //                and tr.RiderId = @riderId
            //                ";
            //using (var sqlConnection = GetTickAndDashConnection())
            //{
            //    return sqlConnection.Query<TripsRiders, CarsTrips, Drivers, Users, CarsQueue, PickupStations, Transportation_Itineraries, TripsRiders>(query,
            //        (tr, ct, d, u, carq, ps, itir) =>
            //        {
            //            d.User = u;
            //            ps.Transportation_Itineraries = itir;
            //            carq.PickupStation = ps;
            //            ct.Driver = d;
            //            ct.CarsQueue = carq;
            //            tr.CarsTrip = ct;
            //            return tr;
            //        },
            //        new { riderId }, splitOn: "Id,UserId,Id,Id,Id,Id").ToList();
            //}

            string query2 = $@"select 
                            carq.Id,
                            tr.CreationDate, 
                            c.Model as 'CarModel',
                            c.RegistrationPlate as 'CarRegistrationPlate',
                             u.Name as 'DriverName',
                             {(language != "ar" ? " transTrans.Name as 'Itinerary' " : " itir.Name as 'Itinerary' ")} 
                            FROM TripsRiders tr, CarsTrips ct, 
                            Drivers d, Users u, CarsQueue carq, PickupStations ps, 
                            Transportation_Itineraries itir, Cars c
                             {(language != "ar" ? " ,TransportationItinerariesTranslations transTrans" : "")} 
                            where tr.TripId = ct.Id
                            and ct.DriverId = d.UserId
                            and u.Id = d.UserId
                            and carq.Id = ct.CarsQueueId
                            and ps.Id = carq.PickupStationId
                            and itir.Id = ps.TransItineraryId
                            and d.CarId = c.Id
                            {(language != "ar" ? "and transTrans.TransportationId = itir.id and transTrans.Lang = '" + language + "'" : "")} 
                            and tr.RiderId = @riderId
                            order by tr.CreationDate desc
                            ";


            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<GetRiderTripsResponse>(query2, new { riderId });
                return result.ToList();
            }
        }

        public IList<TripsRiders> GetAll()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = @"SELECT * FROM TripsRiders tr, Users u, Users u2,
	                                    RidersQueue rq, RidersQStatusLookup rql, PickupStations ps,
	                                    CarsTrips ct 
                                     WHERE tr.RiderId = u.Id AND tr.TripId = ct.Id AND tr.riderQId = rq.Id
		                                AND rq.PickupStationId = ps.Id AND rq.RiderId = u.Id AND rq.RidersQStatusLookupId = rql.Id
		                                AND ct.DriverId = u2.Id";

                var res = connection.Query<TripsRiders, User, User, RidersQueue, RidersQStatusLookup, PickupStations, CarsTrips, TripsRiders>(sql,
                    (tripsRiders, user, user2, riderQ, ridersQStatusLookup, pickupStations, carsTrips) =>
                    {
                        riderQ.PickupStations = pickupStations;
                        
                        riderQ.RidersQStatusLookup = ridersQStatusLookup;

                        carsTrips.User = user2;

                        tripsRiders.CarsTrip = carsTrips;
                        tripsRiders.User = user;
                        tripsRiders.RidersQueue = riderQ;
                        return tripsRiders;
                    },
                    splitOn: "Id, Id, Id, Id, Id"
                    ).ToList();

                return res;
            }
        }

        public async Task<bool> IsRiderQTripExistAsync(int riderqId)
        {
            string query = @"SELECT Id FROM TripsRiders
                              WHERE riderQId = @riderqId";

            using (var cn = GetTickAndDashConnection())
            {
                var res = await cn.QueryAsync<int>(query, new { riderqId });

                return res.FirstOrDefault() > 0;
            }
        }
    }
}
