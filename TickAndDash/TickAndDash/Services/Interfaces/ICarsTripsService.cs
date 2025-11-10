using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface ICarsTripsService
    {
        Task<int> GetRiderQIdFromLastCarTripAsync(string carCode, int riderId);

        Task<List<RidersTickets>> GetLastCarTripInfoAsync(int carId);
    }
}
