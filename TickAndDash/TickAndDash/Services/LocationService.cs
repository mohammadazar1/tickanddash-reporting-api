using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.HttpClients.Interfaces;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class LocationService : ILocationService
    {
        private readonly IGeocodingClient _geocodingClient;
        private readonly ITransItinerariesDAL _transItinerariesDAL;
        private readonly ISitesDAL _sitesDAL;
        private readonly IUsersDAL _usersDAL;
        private readonly IActionContextAccessor _actionContextAccessor;
        //private readonly string _language;

        public LocationService(IGeocodingClient geocodingClient, ITransItinerariesDAL transItinerariesDAL, ISitesDAL sitesDAL, IUsersDAL usersDAL, IActionContextAccessor actionContextAccessor)
        {
            _geocodingClient = geocodingClient;
            _transItinerariesDAL = transItinerariesDAL;
            _sitesDAL = sitesDAL;
            _usersDAL = usersDAL;
            _actionContextAccessor = actionContextAccessor;
          
        }

        public async Task<List<Site>> GetAllSitesThasHasMainPickupStationAsync(bool isActive, int userId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            /* await  _usersDAL.GetUserLanguageAsync(userId);*/
            return await _sitesDAL.GetAllSitesThasHasMainPickupStationAsync(isActive, _language);
        }

        public async Task<string> GetCityBasedOnCoordinates(decimal lat, decimal lng)
        {
            throw new NotImplementedException();
            var response = await _geocodingClient.GetPlaceAddressForCoordinates(lat, lng);
            string city = null;

            if (response.ThirdPartyStatusCode == System.Net.HttpStatusCode.OK)
            {
                // Do some things
                city = response.ThirdPartyResponse;
            }
            else
            {
            }
            return city;
        }


        public List<Site> GetCityIternariesEndPointCities(int cityId)
        {
            throw new NotImplementedException();

            //return _transItinerariesDAL.GetCityIternariesEndPointCities(cityId);
        }

        public async Task<List<Site>> GetSitesToVisitGoFromSpecificSiteAsync(int siteId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            return await _transItinerariesDAL.GetSitesToVisitGoFromSpecificSiteAsync(siteId, _language);
        }
    }
}
