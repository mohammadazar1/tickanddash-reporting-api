using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class UserDAL : BaseDAL, IUsersDAL
    {

        private readonly string _tableName = "Users";
        private readonly string _schema = "dbo";
        
        public UserDAL(IConfiguration configuration) : base(configuration)
        {

        }


        public async Task<int> AddUserAsync(User user)
        {
            string query = $@"INSERT INTO [{_schema}].[{_tableName}] (Name ,CreationDate ,RoleId, Language) 
                         VALUES(@Name,@CreationDate, @RoleId, 'ar'); 
                         SELECT SCOPE_IDENTITY()
                        ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { user.Name, user.CreationDate, user.RoleId });

                return result.FirstOrDefault();
            }
        }


        public async Task<User> GetUserAsync(int id, bool isActive)
        {
            string query = $@"SELECT * 
                        FROM  [{_schema}].[{_tableName}]
                        where id = @id";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<User>(query, new { id });

                return result.FirstOrDefault();
            }
        }

        //public Users GetUserByMobileNumber(Users users)
        //{
        //    string query = $@"SELECT * 
        //                FROM  [{_schema}].[{_tableName}]
        //                where MobileNumber = @MobileNumber";
        //    try
        //    {
        //        using (var sqlConnection = GetTickAndDashConnection())
        //        {
        //            return sqlConnection.Query<Users>(query, new { users.MobileNumber }).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}

        public async Task<User> GetUserAsync(string mobileNumber, bool isActive)
        {
            string query = $@"SELECT * 
                        FROM  [{_schema}].[{_tableName}]
                        where MobileNumber = @mobileNumber
                        and IsActive = @isActive
                        ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<User>(query,
                    new { mobileNumber, isActive });

                return result.FirstOrDefault();
            }
        }

        //public bool UpdateUserRefreshToken(Users user)
        //{
        //    string query = $@"  update dbo.Users set RefreshToken = @RefreshToken
        //                    where id = @Id";

        //    using (var sqlConnection = GetTickAndDashConnection())
        //    {
        //        int affectedRows = sqlConnection.Execute(query, new { user.Id, user.RefreshToken });

        //        return affectedRows > 0;
        //    }
        //}

        public async Task<bool> UpdateUserAsync(int userId, bool isActive)
        {
            string query = $@"  update dbo.Users set IsActive = @isActive
                            where id = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                int affectedRows = await sqlConnection.ExecuteAsync(query, new { userId, isActive });

                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, string Name)
        {
            string query = $@"  update dbo.Users set Name = ISNULL(NULLIF(@Name,''), Name)
                            where id = @userId; ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, Name }) > 0;
            }
        }

        public async Task<bool> UpdateRiderAsync(int userId, char gender)
        {
            string query = $@"Update Riders set Gender = @gender
                              where userId = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, gender }) > 0;
            }
        }

        public bool UpdateDriverPassword(int userId, string password)
        {
            string query = $@"Update Drivers set password = ISNULL(NULLIF(@password,''), password) 
                              where userId = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                int affectedRows = sqlConnection.Execute(query, new { userId, password });

                return affectedRows > 0;
            }
        }

        public async Task<bool> UpdateUserFCMTokenAsync(int userId, string FCMToken)
        {
            string query = $@"update dbo.Users set FCMToken = @FCMToken
                            where id = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, FCMToken }) > 0;
            }
        }

        public async Task<bool> UpdateUserTokenAsync(int userId, string token)
        {
            string query = $@"update dbo.Users set token = @token
                            where id = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, token }) > 0;
            }

        }

        public async Task<bool> UpdateUserLanguageAsync(int userId, string language)
        {
            string query = $@"  update dbo.Users set Language = @language
                            where id = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, language = language.ToLower() }) > 0;

            }
        }

        public async Task<string> GetUserLanguageAsync(int userId)
        {
            string query = $@"SELECT Language 
                        FROM Users
                        where id = @userId
                        ";
            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<string>(query,
                    new { userId });

                return result.FirstOrDefault();
            }

        }

        public Task<bool> IsDriverActiveAsync(int driverId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByNameAsync(string name)
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"SELECT * FROM Users WHERE Name = @name";

                var result = await connection.QueryAsync<User>(query, new { name });

                return result.FirstOrDefault();
            }
        }

        public async Task<int> CreateManualTicketUserAsync()
        {
            using (var connection = GetTickAndDashConnection())
            {
                var query = $@"INSERT INTO Users(Name, RoleId) 
                                OUTPUT Inserted.ID
                                VALUES('ManualTicket', 5)";

                var result = await connection.ExecuteAsync(query);
                var riderQuery = $@"INSERT INTO Riders(UserId, MobileNumber, GeneratedPincode, Token)
                                VALUES({result}, '00000000', '00', 'Token')";
                return result;
            }
        }


        public Admin GetSupervisorUser()
        {
            string query = $@"
                            SELECT a.*, u.*  FROM Admins a, Users u
                            WHERE a.UserId = u.Id
                            AND u.RoleId = {(int)RolesEnum.Supervisor} 
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<Admin>(query).FirstOrDefault();
            }
        }

        public async Task<int> GetCountOfPinCodesGeneratedAsync(string mobileNumber)
        {
            string query = $@"
                                SELECT count(id) as 'count' FROM UsersPincodes up, Riders r 
                                Where r.MobileNumber = @mobileNumber
                                and up.UserId = r.UserId
                                and convert(date,up.CreatedAt) = convert(date, getDate()) 
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<int>(query, new { mobileNumber })).FirstOrDefault();
            }
        }

        public async Task<Riders> GetRiderByMsisdnAsync(string msisdn)
        {
            string query = @"SELECT * FROM Riders
                              WHERE MobileNumber = @msisdn
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<Riders>(query, new { msisdn })).FirstOrDefault();
            }
        }

        public async Task<User> GetDriverByMsisdn(string msisdn)
        {
            string query = @"SELECT * FROM Drivers
                              WHERE MobileNumber = @msisdn
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return (await sqlConnection.QueryAsync<User>(query, new { msisdn })).FirstOrDefault();
            }
        }
    }
}
