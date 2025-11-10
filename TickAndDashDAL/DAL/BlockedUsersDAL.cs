using Dapper;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDashDAL.DAL
{
    public class BlockedUsersDAL : BaseDAL, IBlockedUsersDAL
    {
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
