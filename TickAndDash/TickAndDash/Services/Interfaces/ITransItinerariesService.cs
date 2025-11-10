using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
   public interface ITransItinerariesService
    {
        Task<int>  GetTransItinerariesBySitesEndPointsAsync(int fromSiteId, int towrdSiteId);
        Task<decimal> GetTransItineraryPriceByPickupStation(int psId);
        Task<List<Transportation_Itineraries>> GetTransportationsIternaryPrices(int transId);

        Task<List<Transportation_Itineraries>> GetTransportation_ItinerariesAsync();
    }
}
