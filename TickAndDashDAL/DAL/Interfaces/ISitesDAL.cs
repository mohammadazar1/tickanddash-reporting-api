using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ISitesDAL
    {
        Task<List<Site>> GetAllSitesThasHasMainPickupStationAsync(bool isActive, string language);
        Task<bool> IsSiteActiveAsync(int siteId);
        bool Insert(Site site);
        bool Update(Site site);
        bool Delete(int Id);
        IList<Site> GetAll();
        IList<SiteLookup> GetAllLookupSites();
    }
}
