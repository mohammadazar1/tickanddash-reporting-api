using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class SitesService : ISitesService
    {
        private ISitesDAL _sitesDAL;

        public SitesService(ISitesDAL sitesDAL)
        {
            _sitesDAL = sitesDAL;
        }

        public bool Create(CreateSiteRequest createSiteRequest)
        {
            return _sitesDAL.Insert(new Site {
                Name  = createSiteRequest.Name,
                SitesTypeLookupId = createSiteRequest.SitesTypeLookupId,
                Description = createSiteRequest.Description,
                Latitude = createSiteRequest.Latitude,
                Longitude = createSiteRequest.Longitude,
                IsActive = createSiteRequest.IsActive,
            });
        }

        public bool Delete(int id)
        {
            return _sitesDAL.Delete(id);
        }

        public IList<Site> GetAll()
        {
            return _sitesDAL.GetAll();
        }

        public IList<SiteLookup> GetAllLookupSites()
        {
            return _sitesDAL.GetAllLookupSites();
        }

        public bool Update(UpdateSiteRequest updateSiteRequest)
        {
            return _sitesDAL.Update(new Site {
                Name = updateSiteRequest.Name,
                SitesTypeLookupId = updateSiteRequest.SitesTypeLookupId,
                Description = updateSiteRequest.Description,
                Latitude = updateSiteRequest.Latitude,
                Longitude = updateSiteRequest.Longitude,
                IsActive = updateSiteRequest.IsActive,
                Id = updateSiteRequest.Id
            });
        }
    }
}
