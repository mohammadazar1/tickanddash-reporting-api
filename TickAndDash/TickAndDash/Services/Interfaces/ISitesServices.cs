using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public interface ISitesServices
    {
        Task<List<Site>> GetSitesNamesAsync(bool isActive);
        Task<bool> IsSiteActiveAsync(int siteId);
    }
}
