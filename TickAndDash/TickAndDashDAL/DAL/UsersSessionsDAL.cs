using Dapper;
using System;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class UsersSessionsDAL : BaseDAL, IUsersSessionsDAL
    {
        public UsersSessionsDAL(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<bool> AddUserSessionAsync(UsersSessions us)
        {
            string query = @"INSERT INTO UsersSessions (UserAgent ,DeviceModel ,OSVersion ,Operation ,IP ,UserId ,LogingDate ) 
                             VALUES (@UserAgent ,@DeviceModel ,@OSVersion ,@Operation ,@IP ,@UserId ,@LogingDate  )";

            using (var sqlConnection = GetTickAndDashConnection())
            {

                return await sqlConnection.ExecuteAsync(query, new { us.UserAgent, us.DeviceModel, us.OSVersion, us.Operation, us.IP, us.UserId, us.LogingDate }) > 0;
            }
        }

        // The last one only 
        public async Task<bool> UpdateUserSessionLogoutAsync(int userId, DateTime logoutDate)
        {
            //string query = @"Update UsersSessions set LogingDate = ''
            //                where UserId = @userId";
            string query = $@"
                             WITH USView AS
                            (
                            SELECT top 1 * FROM UsersSessions
                            where UserId = @userId
                            order by id desc
                            ) 
                            update USView
                            set logoutDate = @logoutDate
                            ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, logoutDate }) > 0;
            }
        }
    }
}
