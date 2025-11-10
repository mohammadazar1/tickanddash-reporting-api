using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TickAndDash.Enums;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDash.Services
{
    public class BlacklistedService : IBlacklistedService
    {

        private readonly IBlacklistedUsersTokensDAL _blacklistedUsersTokens;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IBlockedUsersDAL _blockedUsersDAL;

        public BlacklistedService(IBlacklistedUsersTokensDAL blacklistedUsersTokens, IActionContextAccessor actionContextAccessor, IBlockedUsersDAL blockedUsersDAL)
        {
            _blacklistedUsersTokens = blacklistedUsersTokens;
            _actionContextAccessor = actionContextAccessor;
            _blockedUsersDAL = blockedUsersDAL;
        }

        public async Task<bool> ExpireTokenAsync()
        {
            bool success = false;

            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var user = _actionContextAccessor.ActionContext.HttpContext.User;

            int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            string headerToken = request.Headers["authorization"];


            if (!string.IsNullOrWhiteSpace(headerToken))
            {
                headerToken = Regex.Split(headerToken, "Bearer ", RegexOptions.IgnoreCase)?[1];
            }

            bool isInBL = await _blacklistedUsersTokens.IsTokenInBlackListAsync(headerToken, userId);

            if (isInBL)
            {
                bool isTokenExpired = await _blacklistedUsersTokens.UpdateBlackListedTokenAsync(headerToken, userId, true);

                if (isTokenExpired)
                {
                    success = true;
                }
            }
            else
            {
                bool isBlackListed = await _blacklistedUsersTokens.AddTokenToBlackListAsync(headerToken, userId);

                if (isBlackListed)
                {
                    success = true;
                }
            }

            return success;
        }

        public async Task<bool> ExpireTokenAsync(int userId, string token)
        {
            bool isInBL = await   _blacklistedUsersTokens.IsTokenInBlackListAsync(token, userId);
            bool success = false;

            if (isInBL)
            {
                bool isTokenExpired = await  _blacklistedUsersTokens.UpdateBlackListedTokenAsync(token, userId, true);

                if (isTokenExpired)
                {
                    success = true;
                }
            }
            else
            {
                bool isBlackListed = await _blacklistedUsersTokens.AddTokenToBlackListAsync(token, userId);

                if (isBlackListed)
                {
                    success = true;
                }
            }

            return success;
        }

        public bool IsTokenExpired(string token, int userId)
        {
            return _blacklistedUsersTokens.IsTokenExpired(token, userId);
        }

        public bool IsTokenExpired()
        {
            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var user = _actionContextAccessor.ActionContext.HttpContext.User;

            int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            string headerToken = request.Headers["authorization"];

            var x = _blacklistedUsersTokens.IsTokenExpired(headerToken, userId);

            return x;
        }

        public async Task<bool> IsUserBlockedAsync(int userId)
        {
            return await _blockedUsersDAL.IsUserBlockedAsync(userId);
        }
    }
}
