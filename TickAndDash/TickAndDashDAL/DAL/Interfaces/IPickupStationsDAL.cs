using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IPickupStationsDAL
    {
        Task<List<PickupStations>> GetSitePickupStationsForRangeOfSitesAsync(int[] siteId);
        Task<List<PickupStations>> GetSitePickupStationsByIdAndFromSiteAsync(int fromSite, int transId, string language);
        Task<PickupStations> GetPickupStationByIdAsync(int psId, string language);
        Task<List<PickupStations>> GetPickupStationsEndPointsPickupStationsAsync(int SiteId, int transId, string language);

        Task<List<PickupStations>> GetAllActiveMainPickupStationsAsync();
        Task<List<PickupStations>> GetAllAsync();
        Task<int> InsertAsync(PickupStations pickupStations);
        Task<bool> IsPickupstationActiveAndVaildForTheDriverAsync(int carId, int pickupId);
        Task<bool> IsPickupStationValidAndActiveAsync(int psId);
        Task<bool> IsMajorPickupStationAsync(int psId);
        Task<List<PickupStations>> GetAllPickUpStationsByCarIdAsync(int carId, string language);
        Task<int> GetMinorPickupStationMainStaionId(int psid);
        Task<List<PickupStations>> GetMainItineraryId(int id);
        Task<PickupStations> GetByIdAsync(int destinationMainPickup);
        Task<bool> UpdateAsync(PickupStations pickupStations);
        Task<List<PickupStations>> GetUniqueMinorPickupStations(int carId, string language);
        Task<bool> InsertTranslationAsync(int pickupId, string nameAr, string nameEn);
        Task<bool> UpdatePickupTranslation(int id, string nameAr, string nameEn);
    }
}
