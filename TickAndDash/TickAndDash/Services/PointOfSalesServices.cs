using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class PointOfSalesServices : IPointOfSalesServices
    {
        private readonly IPointOfSalesDAL _pointOfSalesDAL;
        private IHttpContextAccessor _accessor;

        public PointOfSalesServices(IPointOfSalesDAL pointOfSalesDAL, IHttpContextAccessor accessor)
        {
            _pointOfSalesDAL = pointOfSalesDAL;
            _accessor = accessor;
        }

        public async Task<List<PointOfSales>> GetAllPointOfsalesAsync(int siteId)
        {
            string language = _accessor.HttpContext.Request.Headers["Content-Language"];
            return await _pointOfSalesDAL.GetAllPointOfsalesAsync(siteId, language);
        }

        public async Task<List<Site>> GetAllPOSSitesAsync()
        {
            string language = _accessor.HttpContext.Request.Headers["Content-Language"];
            return await _pointOfSalesDAL.GetAllPOSSitesAsync(language);

        }
    }
}
