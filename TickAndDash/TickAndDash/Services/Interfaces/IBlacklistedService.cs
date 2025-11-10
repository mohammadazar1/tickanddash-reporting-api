using System.Threading.Tasks;

namespace TickAndDash.Services.Interfaces
{
    public interface IBlacklistedService
    {
        public bool IsTokenExpired(string token, int userId);
        public bool IsTokenExpired();
        public Task<bool> IsUserBlockedAsync(int userId);
        public Task<bool> ExpireTokenAsync();
        public Task<bool> ExpireTokenAsync(int userId, string token);

    }
}
