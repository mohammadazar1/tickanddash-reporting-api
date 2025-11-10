using System.Collections.Generic;
using System.Threading.Tasks;
using static TickAndDashDAL.DAL.TripsRidersDAL;

namespace TickAndDash.Services.Interfaces
{
    public interface ITripsService
    {
        Task<int> GetCarTripByCarsQueueIDAsync(int carqId);
        Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId);
        //List<TripsRiders> GetRidersTrip(int riderId);
        Task<List<GetRiderTripsResponse>> GetRidersTripAsync(int riderId);
        Task<bool> IsRiderQTripExistAsync(int riderqId);
        Task<bool> AddCarTripAsync(int carId, int carqId, int DriverId);
    }
}
