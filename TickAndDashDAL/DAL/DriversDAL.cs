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
                SELECT 
                    u.Id, u.Name, u.FCMToken, u.Token, 
                    d.UserId, d.Password, d.CarId, d.LicenseNumber, d.MobileNumber,
                    d.Address, d.Token, 
                    c.Id, c.RegistrationPlate, c.Model, c.ModelYear, c.SeatCount, c.CarCode, 
                    Itr.Id, Itr.Name, Itr.Description
                FROM Users U, Drivers D, Cars C, Transportation_Itineraries Itr
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
                SELECT 
                    u.Id, u.Name, u.FCMToken, u.Language,
                    d.UserId, d.Password, d.CarId, d.LicenseNumber, d.Address, d.Token, d.MobileOS, d.MobileNumber,
                    c.Id, c.RegistrationPlate, c.Model, c.ModelYear,
                    Itr.Id, Itr.Name, Itr.Description
                FROM Users U, Drivers D, Cars C, Transportation_Itineraries Itr
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
                FROM [TickAndDash].[{_defaultschema}].[{_driversTable}] d, Cars c, Users u
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
                        VALUES(@DriverName, @RoleId);

                    DECLARE @UserId INT = (SELECT SCOPE_IDENTITY());

                    INSERT INTO {_driversTable} 
                        (UserId, LicenseNumber, Password, CarId, Address, Token, MobileNumber)
                    OUTPUT Inserted.UserId
                        VALUES(@UserId, @LicenseNumber, @Password, @CarId, @Address, @Token, @MobileNumber);";

                return connection.Execute(
                    sql,
                    new
                    {
                        driver.LicenseNumber,
                        driver.Password,
                        driver.CarId,
                        driver.Address,
                        driver.Token,
                        driver.MobileNumber,
                        DriverName = driver.User?.Name,
                        RoleId = driver.User?.RoleId ?? (int)RolesEnum.Driver
                    }
                ) > 0;
            }
        }

        public bool Update(Driver driver)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"
                    UPDATE {_driversTable}
                    SET 
                        LicenseNumber = @LicenseNumber,
                        {(driver.Password == "" ? "" : "Password = @Password,")}
                        CarId = @CarId,
                        Address = @Address,
                        IsActive = @IsActive
                    WHERE UserId = @UserId";

                return connection.Execute(
                    sql,
                    new
                    {
                        driver.UserId,
                        driver.LicenseNumber,
                        driver.Password,
                        driver.CarId,
                        driver.Address,
                        driver.IsActive
                    }
                ) > 0;
            }
        }

        public bool Delete(int userId)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = $@"DELETE FROM {_driversTable} WHERE UserId = @UserId";

                return connection.Execute(sql, new { UserId = userId }) > 0;
            }
        }

        public async Task<bool> IsDriverActiveAsync(int driverId)
        {
            string sql = @"SELECT IsActive FROM Drivers WHERE UserId = @driverId";

            using (var connection = GetTickAndDashConnection())
            {
                var res = await connection.QueryAsync<bool>(sql, new { driverId });
                return res.FirstOrDefault();
            }
        }

        public async Task<int> GetDriverUserIdByMobileNumberAsync(string mobileNumber)
        {
            string sql = @"SELECT UserId FROM Drivers WHERE MobileNumber = @mobileNumber";

            using (var connection = GetTickAndDashConnection())
            {
                var res = await connection.QueryAsync<int>(sql, new { mobileNumber });
                return res.FirstOrDefault();
            }
        }

        public async Task<bool> UpdateDriverMobileOs(int driverId, string mobileOS)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var sql = @"
                    UPDATE Drivers 
                    SET MobileOS = @mobileOS
                    WHERE UserId = @driverId";

                return await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        mobileOS = ((MobileOSEnum)Enum.Parse(typeof(MobileOSEnum), mobileOS, true)).ToString(),
                        driverId
                    }
                ) > 0;
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
