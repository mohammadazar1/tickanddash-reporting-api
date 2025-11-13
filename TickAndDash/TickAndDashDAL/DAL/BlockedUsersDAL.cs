using Dapper;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;

using Microsoft.Extensions.Configuration;
namespace TickAndDashDAL.DAL
{
    public class BlockedUsersDAL : BaseDAL, IBlockedUsersDAL
    {
        public BlockedUsersDAL(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> IsUserBlockedAsync(int userId)
        {
            string query = @"SELECT UserId
                            FROM BlockedUsers
                            where IsBlokced = 1
                            and UserId = @userId";

            using (var sqlConnection = GetTickAndDashConnection())
            {
                var result = await sqlConnection.QueryAsync<int>(query, new { userId });
                return result.FirstOrDefault() > 0;
            }
        }
    }
}
