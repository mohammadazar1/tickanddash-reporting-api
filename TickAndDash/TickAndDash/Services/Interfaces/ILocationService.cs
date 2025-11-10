using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface ILocationService
    {
        Task<List<Site>> GetAllSitesThasHasMainPickupStationAsync(bool isActive, int userId);

        //Task<string> GetCityBasedOnCoordinates(decimal lat, decimal lng);
        //List<Sites> GetCityIternariesEndPointCities(int cityId);

        Task<List<Site>> GetSitesToVisitGoFromSpecificSiteAsync(int siteId);



    }
}
