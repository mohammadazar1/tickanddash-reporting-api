using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public interface IPointOfSalesServices
    {
        Task<List<PointOfSales>> GetAllPointOfsalesAsync(int siteId);

        Task<List<Site>> GetAllPOSSitesAsync();
    }
}
