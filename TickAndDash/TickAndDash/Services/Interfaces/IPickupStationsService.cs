using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IPickupStationsService
    {
        Task<List<PickupStations>> GetSitePickupStationsForRangeOfSitesAsync(int[] SiteId);
        Task<List<PickupStations>> GetSitePickupStationsByIdAndFromSiteAsync(int fromSite, int transId);
        Task<List<MajorsMinorStations>> GetMinorPickupStationsThatFollowsMainPickupStationAsync(int mainPickupStation, string language);
        Task<List<MajorsMinorStations>> GetMinorPickupStationsInSiteWithinSpecificTrans(int fromSiteId, int transId, string language);
        Task<List<PickupStations>> GetAllPickUpStationsByCarIdAsync(int carId);
        Task<List<PickupStations>> GetUniqueMinorPickupStations(int carId);
        Task<PickupStations> GetPickupStationsByIdAsync(int psId);
        Task<int> GetMinorPickupStationMainStaionId(int psid);
        Task<MajorsMinorStations> GetMajorsMinorStationsByMinorStationId(int minorId);

        Task<List<PickupStations>> GetPickupStationsEndPointsPickupStationsAsync(int psSiteId, int transId);
        Task<bool> IsPickupStationValidAndActiveAsync(int psId);
        Task<bool> IsMajorPickupStationAsync(int psId);
        Task<bool> IsPickupstationActiveAndVaildForTheDriverAsync(int carId, int pickupId);
    }
}
