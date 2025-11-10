using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class ItinerariesService : IItinerariesService
    {
        public readonly ITransItinerariesDAL _transItinerariesDAL;

        public ItinerariesService(ITransItinerariesDAL transItinerariesDAL)
        {
            _transItinerariesDAL = transItinerariesDAL;
        }

        public async Task<int> CreateAsync(CreateItineraryRequest createItinerariesRequest)
        {
            int id = await _transItinerariesDAL.Insert(new Transportation_Itineraries { 
                Description = createItinerariesRequest.Description,
                FromSiteId = createItinerariesRequest.FromSiteId,
                IsActive = createItinerariesRequest.IsActive,
                ItineraryTypeLookupId = createItinerariesRequest.ItinerayTypeLookupId,
                Name = createItinerariesRequest.Name,
                Price = createItinerariesRequest.Price,
                TowardSiteId = createItinerariesRequest.TowardSiteId
            });

            return id;
        }

        public IList<Transportation_Itineraries> GetAll(GetAllItinerariesRequset getAllItinerariesRequset)
        {
            return _transItinerariesDAL.GetAll(getAllItinerariesRequset.IsActive);
        }

        public async Task<bool> UpdateAsync(UpdateItineraryRequest updateItineraryRequest)
        {
            bool result = await _transItinerariesDAL.UpdateAsync(new Transportation_Itineraries
            {
                Id = updateItineraryRequest.Id,
                Description = updateItineraryRequest.Description,
                FromSiteId = updateItineraryRequest.FromSiteId,
                IsActive = updateItineraryRequest.IsActive,
                ItineraryTypeLookupId = updateItineraryRequest.ItinerayTypeLookupId,
                Name = updateItineraryRequest.Name,
                Price = updateItineraryRequest.Price,
                TowardSiteId = updateItineraryRequest.TowardSiteId
            });

            if(!result)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Failed to update");

            return result;
        }
    }
}
