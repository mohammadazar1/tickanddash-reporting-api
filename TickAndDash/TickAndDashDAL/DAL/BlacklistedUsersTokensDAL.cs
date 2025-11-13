using Dapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDashDAL.DAL
{
    public class BlacklistedUsersTokensDAL : BaseDAL, IBlacklistedUsersTokensDAL
    {
        public BlacklistedUsersTokensDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> AddTokenToBlackListAsync(string token, int userId)
        {
            string query = @"insert into BlacklistedUsersTokens (Token, IsBlocked, CreationDate, UserId)
                            Values(@Token, @IsBlocked, @CreationDate, @UserId)";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, token, IsBlocked = true, CreationDate = DateTime.Now }) > 0;
            }
        }

        public bool IsTokenExpired(string token, int userId)
        {
            string query = @"select IsBlocked from BlacklistedUsersTokens
                            where UserId = @userId and IsBlocked = 1 and Token = @Token ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return sqlConnection.Query<bool>(query, new { userId, token }).FirstOrDefault();
            }
        }


        public async Task<bool> IsTokenInBlackListAsync(string token, int userId)
        {
            string query = @"select IsBlocked from BlacklistedUsersTokens
                             where UserId = @userId and Token = @Token ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<bool>(query, new { userId, token });
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> UpdateBlackListedTokenAsync(string token, int userId, bool isBlocked)
        {
            string query = @"Update   BlacklistedUsersTokens set IsBlocked = @IsBlocked
                             where UserId = @userId and Token = @Token ";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                return await sqlConnection.ExecuteAsync(query, new { userId, token, isBlocked }) > 0;
            }
        }


    }
}
