using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface IPosService
    {
        Task<IList<PointOfSales>> GetAll();
        Task<bool> CreateAsync(PointOfSales pointOfSales);
        Task<bool> UpdateAsync(PointOfSales pointOfSales);
        Task<bool> DeleteAsync(int id);
        Task<PointOfSales> GetPointOfSaleByIdAsync(int userId);
    }
}
