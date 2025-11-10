using System.Threading.Tasks;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IBlacklistedUsersTokensDAL
    {
        //bool AddBlackListedUserToken(BlacklistedUsersTokens blacklisted);
        bool IsTokenExpired(string token, int userId);
        Task<bool> IsTokenInBlackListAsync(string token, int userId);
        Task<bool> AddTokenToBlackListAsync(string token, int userId);
        Task<bool> UpdateBlackListedTokenAsync(string token, int userId, bool isBlocked);
    }
}
