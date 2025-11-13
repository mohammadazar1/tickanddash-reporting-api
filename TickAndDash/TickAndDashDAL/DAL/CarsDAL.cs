using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class CarsDAL : BaseDAL, ICarsDAL
    {

        private const string CarsTable = "Cars";

        public CarsDAL(IConfiguration configuration) : base(configuration) { }

        public bool Create(Car car)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"INSERT INTO {CarsTable} (IsActive, ItineraryId, RegistrationPlate, 
                                Model, ModelYear, LoggedInDriverId, SeatCount, CarCode)
                                VALUES  (@IsActive, @ItineraryId, @RegistrationPlate, 
                                    @Model, @ModelYear, @LoggedInDriverId, @SeatCount, @CarCode)";

                return connection.Execute(sql,
                    new
                    {
                        car.IsActive,
                        car.ItineraryId,
                        car.RegistrationPlate,
                        car.Model,
                        car.ModelYear,
                        car.LoggedInDriverId,
                        car.SeatCount,
                        car.CarCode
                    }) > 0;
            }
        }

        public bool DeleteCar(int id)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"DELETE {CarsTable} WHERE Id = @Id";

                return connection.Execute(sql,
                    new { Id = id }) > 0;
            }
        }

        public List<Car> GetAll()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"SELECT * FROM Cars c,  [Transportation_Itineraries] ti, Users u,  ItineraryTypeLookup it
	                            WHERE ti.Id = c.ItineraryId and c.LoggedInDriverId = u.Id and ti.ItineraryTypeLookupId = it.Id;";

                var sql2 = $@"SELECT * FROM Cars c,  [Transportation_Itineraries] ti, ItineraryTypeLookup it
	                            WHERE ti.Id = c.ItineraryId and ti.ItineraryTypeLookupId = it.Id  and c.LoggedInDriverId = 0;";

                var res1 = connection.Query<Car, Transportation_Itineraries, User, ItineraryTypeLookup, Car>(sql,
                    (car, transportation_Itineraries, user, itineraryTypeLookup) =>
                    {
                        transportation_Itineraries.ItineraryTypeLookup = itineraryTypeLookup;
                        car.Transportation_Itineraries = transportation_Itineraries;
                        car.User = user;
                        return car;
                    },
                    splitOn: "Id, Id, Id"
                    ).ToList();

                var res2 = connection.Query<Car, Transportation_Itineraries, ItineraryTypeLookup, Car>(sql2,
                    (car, transportation_Itineraries, itineraryTypeLookup) =>
                    {
                        transportation_Itineraries.ItineraryTypeLookup = itineraryTypeLookup;
                        car.Transportation_Itineraries = transportation_Itineraries;
                        return car;
                    },
                    splitOn: "Id, Id"
                    ).ToList();

                res2.ForEach(car => res1.Add(car));

                return res1;
            }
        }

        public async Task<int> GetCarActiveDriverIdByCarIdAsync(int id)
        {
            string query = $@"select LoggedInDriverId from Cars
                               where id = @id";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { id });

                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetCarDriversCountAsync(int carId)
        {
            string query = $@" select count(UserId) from Drivers
                                    where CarId =@carId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carId });
                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetCarCountOfSeatsAsync(int carId)
        {
            string query = $@"select SeatCount from Cars
                               where id = @carId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { carId });

                return result.FirstOrDefault();
            }
        }

        public bool Insert(Car car)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> IsCarActiveAsync(int carId)
        {
            string query = $@"select isActive from Cars
                               where id = @carId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<bool>(query, new { carId });

                return result.FirstOrDefault();
            }
        }

        public bool Update(Car car)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"UPDATE {CarsTable} 
                                SET  IsActive = @IsActive, ItineraryId = @ItineraryId, RegistrationPlate = @RegistrationPlate, 
                                    Model = @Model, ModelYear = @ModelYear, LoggedInDriverId = @LoggedInDriverId, SeatCount = @SeatCount
                                WHERE Id = @Id";

                return connection.Execute(sql,
                    new
                    {
                        car.IsActive,
                        car.ItineraryId,
                        car.RegistrationPlate,
                        car.Model,
                        car.ModelYear,
                        car.LoggedInDriverId,
                        car.SeatCount,
                        car.Id
                    }) > 0;
            }
        }

        public async Task<bool> UpdateLoggedInCarUserAsync(int carId, int driverId)
        {
            string query = $@"update Cars set LoggedInDriverId = @driverId
                            where Id = @carId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { carId, driverId }) > 0;
            }
        }

        public async Task<bool> IsCarActiveByCarCodeAsync(string carCode)
        {
            string query = @"SELECT isActive FROM Cars
                            WHERE CarCode = @carCode
                            and IsActive = 1";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<bool>(query, new { carCode });
            }
        }

        public async Task<int> GetCarIdByCarCodeAsync(string carCode)
        {
            string query = @"SELECT id FROM Cars
                            WHERE CarCode = @carCode";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.QueryFirstOrDefaultAsync<int>(query, new { carCode });
            }
        }
    }
}
