using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class PickupStationsService : IPickupStationsService
    {
        private readonly IPickupStationsDAL _pickupStationsDAL;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IMajorsMinorStationsDAL _minorPickupStationsDAL;

        public PickupStationsService(IPickupStationsDAL pickupStationsDAL,
            IActionContextAccessor actionContextAccessor, IMajorsMinorStationsDAL minorPickupStationsDAL)
        {
            _pickupStationsDAL = pickupStationsDAL;
            _actionContextAccessor = actionContextAccessor;
            _minorPickupStationsDAL = minorPickupStationsDAL;
        }

        public async Task<List<PickupStations>> GetAllPickUpStationsByCarIdAsync(int carId)
        {
            string language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];

            return await _pickupStationsDAL.GetAllPickUpStationsByCarIdAsync(carId, language);
        }

        public async Task<List<PickupStations>> GetUniqueMinorPickupStations(int carId)
        {
            string language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            return await _pickupStationsDAL.GetUniqueMinorPickupStations(carId, language);
        }

        public async Task<MajorsMinorStations> GetMajorsMinorStationsByMinorStationId(int minorId)
        {
            return await _minorPickupStationsDAL.GetMajorsMinorStationsByMinorStationId(minorId);
        }

        public async Task<int> GetMinorPickupStationMainStaionId(int psid)
        {
            return await _pickupStationsDAL.GetMinorPickupStationMainStaionId(psid);
        }

        public async Task<List<MajorsMinorStations>> GetMinorPickupStationsInSiteWithinSpecificTrans(int fromSiteId, int transId, string language)
        {
            return await _minorPickupStationsDAL.GetMinorPickupStationsInSiteWithinSpecificTrans(fromSiteId, transId, language);
        }

        public async Task<List<MajorsMinorStations>> GetMinorPickupStationsThatFollowsMainPickupStationAsync(
            int mainPickupStation,
            string language)
        {
            return await _minorPickupStationsDAL.GetMinorPickupStationsThatFollowsMainPickupStationAsync(mainPickupStation, language);
        }

        public async Task<PickupStations> GetPickupStationsByIdAsync(int psId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            return await _pickupStationsDAL.GetPickupStationByIdAsync(psId, _language);
        }

        public async Task<List<PickupStations>> GetPickupStationsEndPointsPickupStationsAsync(int psSiteId, int transId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];

            return await _pickupStationsDAL.GetPickupStationsEndPointsPickupStationsAsync(psSiteId, transId, _language);
        }

        public async Task<List<PickupStations>> GetSitePickupStationsByIdAndFromSiteAsync(int fromSite, int transId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];

            return await _pickupStationsDAL.GetSitePickupStationsByIdAndFromSiteAsync(fromSite, transId, _language);
        }

        public async Task<List<PickupStations>> GetSitePickupStationsForRangeOfSitesAsync(int[] SiteId)
        {
            return await _pickupStationsDAL.GetSitePickupStationsForRangeOfSitesAsync(SiteId);
        }

        public async Task<bool> IsMajorPickupStationAsync(int psId)
        {
            return await _pickupStationsDAL.IsMajorPickupStationAsync(psId);
        }

        public async Task<bool> IsPickupstationActiveAndVaildForTheDriverAsync(int carId, int pickupId)
        {
            return await _pickupStationsDAL.IsPickupstationActiveAndVaildForTheDriverAsync(carId, pickupId);
        }

        public async Task<bool> IsPickupStationValidAndActiveAsync(int psId)
        {
            return await _pickupStationsDAL.IsPickupStationValidAndActiveAsync(psId);
        }
    }
}
