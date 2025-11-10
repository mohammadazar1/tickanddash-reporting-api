using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;

namespace TickAndDashReportingTool.Services
{
    public interface IPickupStationsService
    {
        Task<List<PickupStations>> GetAll();
        Task<int> Create(CreatePickupStationRequest createPickupStationRequest);
        Task<List<PickupStations>> GetMainByItineriesId(int id);
        Task<bool> Update(UpdatePickupRequest updatePickupRequest);
    }
}