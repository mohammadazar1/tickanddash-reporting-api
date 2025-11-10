using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class TransItinerariesService : ITransItinerariesService
    {
        private readonly ITransItinerariesDAL _itinerariesDAL;
        private readonly IHttpContextAccessor _accessor;

        public TransItinerariesService(ITransItinerariesDAL itinerariesDAL, IHttpContextAccessor accessor)
        {
            _itinerariesDAL = itinerariesDAL;
            _accessor = accessor;
        }

        public async Task<int> GetTransItinerariesBySitesEndPointsAsync(int fromSiteId, int towrdSiteId)
        {
            return await _itinerariesDAL.GetTransItinerariesBySitesEndPointsAsync(fromSiteId, towrdSiteId);
        }

        public async Task<decimal> GetTransItineraryPriceByPickupStation(int psId)
        {
            return await _itinerariesDAL.GetTransItineraryPriceByPickupStation(psId);
        }

        public async Task<List<Transportation_Itineraries>> GetTransportationsIternaryPrices(int transId)
        {
            string language = _accessor.HttpContext.Request.Headers["Content-Language"];
            return await _itinerariesDAL.GetTransportationsIternaryPrices(transId, language);
        }

        public async Task<List<Transportation_Itineraries>> GetTransportation_ItinerariesAsync()
        {
            return await _itinerariesDAL.GetTransportation_ItinerariesAsync();
        }
    }
}
