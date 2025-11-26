using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class DriversDAL : BaseDAL, IDriversDAL
    {
        private readonly string _driversTable = "Drivers";
        private readonly string _defaultschema = "dbo";

        public DriversDAL() : base()
        {
        }

        public async Task<Driver> GetDriverByLicenseNumberAsync(string licenseNumber)
        {
            string query = $@"
                SELECT u.Id, u.Name, u.FCMToken, u.Token,
                       d.UserId, d.Password, d.CarId, d.LicenseNumber,
                       d.MobileNumber, d.Address, d.Token,
                       c.Id, c.RegistrationPlate, c.Model, c.ModelYear,
                       c.SeatCount, c.CarCode,
                       Itr.Id, Itr.Name, Itr.Description
                FROM Users u, Drivers d, Cars c, Transportation_Itineraries Itr
                WHERE u.Id = d.UserId
                AND d.CarId = c.Id
                AND c.ItineraryId = Itr.Id
                AND d.LicenseNumber = @licenseNumber COLLATE SQL_Latin1_General_CP1_CI_AS";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<User, Driver, Car, Transportation_Itineraries, Driver>(
                    query,
                    (u, d, c, itr) =>
                    {
                        c.Transportation_Itineraries = itr;
                        d.Car = c;
                        d.User = u;
                        d.UserId = u.Id;
                        return d;
                    },
                    new { licenseNumber },
                    splitOn: "UserId, Id, Id"
                );

                return result.FirstOrDefault();
            }
        }

        public async Task<Driver> GetDriverByUserIdAsync(int userId)
        {
            string query = $@"
                SELECT u.Id, u.Name, u.FCMToken, u.Language,
                       d.UserId, d.Password, d.CarId, d.LicenseNumber,
                       d.Address, d.Token, d.MobileOS, d.MobileNumber,
                       c.Id, c.RegistrationPlate, c.Model, c.ModelYear,
                       Itr.Id, Itr.Name, Itr.Description
                FROM Users u, Drivers d, Cars c, Transportation_Itineraries Itr
                WHERE u.Id = d.UserId
                AND d.CarId = c.Id
                AND c.ItineraryId = Itr.Id
                AND d.UserId = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<User, Driver, Car, Transportation_Itineraries, Driver>(
                    query,
                    (u, d, c, itr) =>
                    {
                        c.Transportation_Itineraries = itr;
                        d.Car = c;
                        d.User = u;
                        return d;
                    },
                    new { userId },
                    splitOn: "UserId, Id, Id"
                );

                return result.FirstOrDefault();
            }
        }

        public List<Driver> GetDrivers()
        {
            string query = $@"
                SELECT * 
                FROM Drivers d, Cars c, Users u
                WHERE d.CarId = c.Id 
                AND d.UserId = u.Id";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<Driver, Car, User, Driver>(
                    query,
                    (driver, car, user) =>
                    {
                        driver.Car = car;
                        driver.User = user;
                        return driver;
                    },
                    splitOn: "Id, Id"
                ).ToList();
            }
        }

        public bool Insert(Driver driver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    INSERT INTO Users (Name, RoleId)
                    VALUES(@DriverName, 3)

                    DECLARE @UserId INT = SCOPE_IDENTITY()

                    INSERT INTO Drivers (UserId, LicenseNumber, Password, CarId, Address, Token, MobileNumber)
                    VALUES(@UserId, @LicenseNumber, @Password, @CarId, @Address, @Token, @MobileNumber)

                    SELECT @UserId";

                var newUserId = connection.ExecuteScalar<int>(sql,
                    new
                    {
                        DriverName = driver.User.Name,
                        driver.LicenseNumber,
                        driver.Password,
                        driver.CarId,
                        driver.Address,
                        driver.Token,
                        driver.MobileNumber
                    });

                driver.UserId = newUserId;
                return newUserId > 0;
            }
        }

        public bool Update(Driver driver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    UPDATE Drivers
                    SET LicenseNumber = @LicenseNumber,
                        {(string.IsNullOrEmpty(driver.Password) ? "" : "Password = @Password,")}
                        CarId = @CarId,
                        Address = @Address,
                        IsActive = @IsActive
                    WHERE UserId = @UserId";

                return connection.Execute(sql, new
                {
                    driver.UserId,
                    driver.LicenseNumber,
                    driver.Password,
                    driver.CarId,
                    driver.Address,
                    driver.IsActive
                }) > 0;
            }
        }

        public bool Delete(int userId)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"DELETE FROM Drivers WHERE UserId = @UserId";

                return connection.Execute(sql, new { userId }) > 0;
            }
        }

        public async Task<bool> IsDriverActiveAsync(int driverId)
        {
            string sql = "SELECT IsActive FROM Drivers WHERE UserId = @driverId";

            using (var connection = GetTickAndDashConnection())
            {
                return (await connection.QueryAsync<bool>(sql, new { driverId })).FirstOrDefault();
            }
        }

        public async Task<int> GetDriverUserIdByMobileNumberAsync(string mobileNumber)
        {
            string sql = "SELECT UserId FROM Drivers WHERE MobileNumber = @mobileNumber";

            using (var connection = GetTickAndDashConnection())
            {
                return (await connection.QueryAsync<int>(sql, new { mobileNumber })).FirstOrDefault();
            }
        }

        public async Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS)
        {
            using (var connection = GetTickAndDashConnection())
            {
                string sql = @"UPDATE Drivers SET MobileOS = @mobileOS WHERE UserId = @driverId";

                return await connection.ExecuteAsync(sql, new
                {
                    mobileOS = ((MobileOSEnum)Enum.Parse(typeof(MobileOSEnum), mobileOS, true)).ToString(),
                    driverId
                }) > 0;
            }
        }

        public async Task<Driver> GetDriverByCarIdAsync(int carId)
        {
            string sql = @"
                SELECT d.UserId, u.Id, u.Language, u.FCMToken
                FROM Drivers d, Cars c, Users u
                WHERE c.LoggedInDriverId = d.UserId
                AND u.Id = d.UserId
                AND c.Id = @carId";

            using (var connection = GetTickAndDashConnection())
            {
                return (await connection.QueryAsync<Driver, User, Driver>(
                    sql,
                    (d, u) =>
                    {
                        d.User = u;
                        return d;
                    },
                    new { carId },
                    splitOn: "Id"
                )).FirstOrDefault();
            }
        }
    }
}
