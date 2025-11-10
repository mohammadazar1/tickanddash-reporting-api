using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class PosService : IPosService
    {
        private readonly IPointOfSalesDAL _pointOfSalesDAL;

        public PosService(IPointOfSalesDAL pointOfSalesDAL)
        {
            _pointOfSalesDAL = pointOfSalesDAL;
        }

        public async Task<bool> CreateAsync(PointOfSales pointOfSales)
        {
            return await _pointOfSalesDAL.InsertAsync(pointOfSales);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            bool d = await _pointOfSalesDAL.DeleteAsync(id);
            return d;
        }

        public async Task<IList<PointOfSales>> GetAll()
        {
            return await _pointOfSalesDAL.GetAllPointOfsalesAsync(0, "ar");
        }


        //public Task<PointOfSales> GetPointOfSale(int userId)
        //{
        //    throw new System.NotImplementedException();
        //}

        public async Task<PointOfSales> GetPointOfSaleByIdAsync(int userId)
        {
            return await _pointOfSalesDAL.GetPointOfSaleByIdAsync(userId);
       }


        public async Task<bool> UpdateAsync(PointOfSales pointOfSales)
        {
            return await _pointOfSalesDAL.UpdateAsync(pointOfSales);
        }
    }
}
