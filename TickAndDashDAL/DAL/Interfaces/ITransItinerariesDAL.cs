using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ITransItinerariesDAL
    {
        Task<List<Site>> GetSitesToVisitGoFromSpecificSiteAsync(int siteId, string language);
        Task<int> GetTransItinerariesBySitesEndPointsAsync(int fromSiteId, int towrdSiteId);
        IList<Transportation_Itineraries> GetAll(bool? isActive);
        Task<decimal> GetTransItineraryPriceByPickupStation(int psId);
        Task<int> Insert(Transportation_Itineraries transportation_Itineraries);
        Task<List<Transportation_Itineraries>> GetTransportationsIternaryPrices(int transId, string lang);
        Task<List<Transportation_Itineraries>> GetTransportation_ItinerariesAsync();
        Task<bool> UpdateAsync(Transportation_Itineraries transportation_Itineraries);
        Transportation_Itineraries GetById(int transItineraryId);
    }
}
