using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class SitesServices : ISitesServices
    {
        private readonly ISitesDAL _sitesDAL;
        private readonly IActionContextAccessor _actionContextAccessor;

        public SitesServices(ISitesDAL sitesDAL, IActionContextAccessor actionContextAccessor)
        {
            _sitesDAL = sitesDAL;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<List<Site>> GetSitesNamesAsync(bool isActive)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];

            return await _sitesDAL.GetAllSitesThasHasMainPickupStationAsync(isActive, _language);
        }

        public async Task<bool> IsSiteActiveAsync(int siteId)
        {
            return await _sitesDAL.IsSiteActiveAsync(siteId);
        }
    }
}
