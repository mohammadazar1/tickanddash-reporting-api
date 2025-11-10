using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Exceptions;

namespace TickAndDashReportingTool.Services
{
    public class PickupStationsService : IPickupStationsService
    {
        private readonly IPickupStationsDAL _pickupStationsDAL;
        private readonly IMajorsMinorStationsDAL _majorsMinorStationsDAL;

        public PickupStationsService(IPickupStationsDAL pickupStationsDAL, IMajorsMinorStationsDAL majorsMinorStationsDAL)
        {
            _pickupStationsDAL = pickupStationsDAL;
            _majorsMinorStationsDAL = majorsMinorStationsDAL;
        }

        public async Task<int> Create(CreatePickupStationRequest createPickupStationRequest)
        {
            int pickupId = await _pickupStationsDAL.InsertAsync(new PickupStations
            {
                IsActive = createPickupStationRequest.IsActive,
                Latitude = createPickupStationRequest.Latitude,
                Longitude = createPickupStationRequest.Longitude,
                Radius = createPickupStationRequest.Radius,
                SiteId = createPickupStationRequest.SiteId,
                TransItineraryId = createPickupStationRequest.TransItineraryId,
                Descriptions = createPickupStationRequest.Description,
                PickupTypeId = createPickupStationRequest.PickupTypeId
            });

            bool inserted = await _pickupStationsDAL.InsertTranslationAsync(pickupId,
                                                                            createPickupStationRequest.NameAr,
                                                                            createPickupStationRequest.NameEn);
            
            if (createPickupStationRequest.PickupTypeId == (int)PickupStationsLookupEnum.Minor)
            {
                PickupStations mainPickup = await _pickupStationsDAL.GetByIdAsync(createPickupStationRequest.DestinationMainPickup);
                bool majorsMinorStation = await _majorsMinorStationsDAL.InsertAsync(new MajorsMinorStations
                {
                    MainPickupStationId = createPickupStationRequest.DestinationMainPickup,
                    MinorPickupStationId = pickupId,
                    FromSiteId = mainPickup.Id,
                    TransItineraryId = createPickupStationRequest.TransItineraryId,
                    TowardSiteId = createPickupStationRequest.To

                });
            }

            return pickupId;
        }

        public async Task<List<PickupStations>> GetAll()
        {
            List<PickupStations> pickupStations = await _pickupStationsDAL.GetAllAsync();

            return pickupStations;
        }

        public async Task<List<PickupStations>> GetMainByItineriesId(int id)
        {
            List<PickupStations> pickupStations = await _pickupStationsDAL.GetMainItineraryId(id);

            return pickupStations;
        }

        public async Task<bool> Update(UpdatePickupRequest updatePickupRequest)
        {
            bool result = await _pickupStationsDAL.UpdateAsync(new PickupStations
            {
                Id = updatePickupRequest.Id,
                IsActive = updatePickupRequest.IsActive,
                Latitude = updatePickupRequest.Latitude,
                Longitude = updatePickupRequest.Longitude,
                Radius = updatePickupRequest.Radius,
                TransItineraryId = updatePickupRequest.TransItineraryId,
            });

            bool result2 = await _pickupStationsDAL.UpdatePickupTranslation(updatePickupRequest.Id,
                                                                            updatePickupRequest.NameAr,
                                                                            updatePickupRequest.NameEn);
            if (!result) throw new HttpStatusException(HttpStatusCode.BadRequest, "Failed to update");

            return result;
        }
    }
}
