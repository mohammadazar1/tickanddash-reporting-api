using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;

namespace TickAndDashReportingTool.Services.Interfaces
{
    public interface ISitesService
    {
        bool Create(CreateSiteRequest createSiteRequest);
        IList<Site> GetAll();
        IList<SiteLookup> GetAllLookupSites();
        bool Update(UpdateSiteRequest updateSiteRequest);
        bool Delete(int id);
    }
}
