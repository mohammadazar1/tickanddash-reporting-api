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

        // ---------------------------------------------------------
        // GET BY LICENSE
        // ---------------------------------------------------------
        public async Task<Driver> GetDriverByLicenseNumberAsync(string licenseNumber)
        {
            string query = $@"
                SELECT 
                    u.Id, u.Name, u.UserName, u.Password, u.FCMToken, u.Token, 
                    d.UserId, d.Password, d.CarId, d.LicenseNumber, d.MobileNumber,
                    d.Address, d.token, 
                    c.Id, c.RegistrationPlate, c.Model, c.ModelYear, c.seatCount, c.CarCode, 
                    Itr.Id, Itr.Name, Itr.Description
                FROM Users U, Drivers D, Cars C, Transportation_Itineraries Itr
                WHERE u.Id = d.UserId AND d.CarId = c.Id AND c.ItineraryId = Itr.Id
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

        // ---------------------------------------------------------
        // GET BY USER ID
        // ---------------------------------------------------------
        public async Task<Driver> GetDriverByUserIdAsync(int userId)
        {
            string query = $@"
                SELECT 
                    u.Id, u.Name, u.UserName, u.Password, u.FCMToken, u.language,
                    d.UserId, d.Password, d.CarId, d.LicenseNumber, D.Address, d.token, d.MobileOS, d.MobileNumber,
                    c.Id, c.RegistrationPlate, c.Model, c.ModelYear, 
                    Itr.Id, Itr.Name, Itr.Description
                FROM Users U, Drivers D, Cars C, Transportation_Itineraries Itr
                WHERE u.Id = d.UserId AND d.CarId = c.Id AND c.ItineraryId = Itr.Id
                  AND d.userId = @userId";

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

        // ---------------------------------------------------------
        // GET ALL DRIVERS
        // ---------------------------------------------------------
        public List<Driver> GetDrivers()
        {
            string query = $"SELECT * FROM [TickAndDash].[{_defaultschema}].[{_driversTable}] d, Cars c, Users u " +
                           $"WHERE d.CarId = c.Id AND d.UserId = u.Id";

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

        // ---------------------------------------------------------
        // INSERT DRIVER + FULL USER
        // ---------------------------------------------------------
        public bool Insert(Driver driver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    INSERT INTO Users (Name, UserName, Password, RoleId, IsActive)
                    VALUES(@Name, @UserName, @Password, 3, 1);

                    DECLARE @UserId INT = SCOPE_IDENTITY();

                    INSERT INTO Drivers (UserId, LicenseNumber, Password, CarId, Address, Token, MobileNumber)
                    VALUES(@UserId, @LicenseNumber, @Password, @CarId, @Address, @Token, @MobileNumber);
                ";

                return connection.Execute(sql, new
                {
                    Name = driver.User.Name,
                    UserName = driver.LicenseNumber,   // important
                    Password = driver.Password,
                    driver.LicenseNumber,
                    driver.CarId,
                    driver.Address,
                    driver.Token,
                    driver.MobileNumber
                }) > 0;
            }
        }

        // ---------------------------------------------------------
        // UPDATE DRIVER + USER
        // ---------------------------------------------------------
        public bool Update(Driver driver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    UPDATE Users
                    SET Name = @Name,
                        UserName = @UserName,
                        {(driver.Password == "" ? "" : "Password = @Password,")}
                        IsActive = @IsActive
                    WHERE Id = @UserId;

                    UPDATE Drivers
                    SET LicenseNumber = @LicenseNumber,
                        {(driver.Password == "" ? "" : "Password = @Password,")}
                        CarId = @CarId,
                        Address = @Address,
                        IsActive = @IsActive
                    WHERE UserId = @UserId;
                ";

                return connection.Execute(sql, new
                {
                    driver.UserId,
                    driver.LicenseNumber,
                    driver.Password,
                    driver.CarId,
                    driver.Address,
                    driver.IsActive,
                    Name = driver.User.Name,
                    UserName = driver.User.UserName
                }) > 0;
            }
        }

        // ---------------------------------------------------------
        // DELETE DRIVER + USER
        // ---------------------------------------------------------
        public bool Delete(int userId)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    DELETE FROM Drivers WHERE UserId = @UserId;
                    DELETE FROM Users WHERE Id = @UserId;
                ";

                return connection.Execute(sql, new { userId }) > 0;
            }
        }

        // ---------------------------------------------------------
        public async Task<bool> IsDriverActiveAsync(int driverId)
        {
            string sql = $@"SELECT IsActive FROM Drivers WHERE UserId = @driverId";

            using (var connection = GetTickAndDashConnection())
            {
                return (await connection.QueryAsync<bool>(sql, new { driverId })).FirstOrDefault();
            }
        }

        public async Task<int> GetDriverUserIdByMobileNumberAsync(string mobileNumber)
        {
            string sql = $@"SELECT UserId FROM Drivers WHERE MobileNumber = @mobileNumber";

            using (var connection = GetTickAndDashConnection())
            {
                return (await connection.QueryAsync<int>(sql, new { mobileNumber })).FirstOrDefault();
            }
        }

        public async Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = @"UPDATE Drivers SET MobileOS = @mobileOS WHERE UserId = @driverId";

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
