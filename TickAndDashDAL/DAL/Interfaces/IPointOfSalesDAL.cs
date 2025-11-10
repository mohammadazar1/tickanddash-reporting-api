using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IPointOfSalesDAL
    {
        Task<List<PointOfSales>> GetAllPointOfsalesAsync(int siteId, string language);

        //Task<PointOfSales> GetPointOfSale
        Task<List<Site>> GetAllPOSSitesAsync(string language);
        Task<bool> InsertAsync(PointOfSales pointOfSales);
        Task<bool> UpdateAsync(PointOfSales pointOfSales);
        Task<bool> DeleteAsync(int id);
        PointOfSales GetPOSByUsername(string username);
        Task<PointOfSales> GetPointOfSaleByUserIdAsync(int userId);
        Task<PointOfSales> GetPointOfSaleByIdAsync(int userId);
     }
}
