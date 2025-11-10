using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;
using static TickAndDashDAL.DAL.TripsRidersDAL;

namespace TickAndDashDAL.DAL
{
    public interface ITripsRidersDAL
    {
        Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId);
        //List<TripsRiders> GetRidersTrip(int riderId);
        Task<List<GetRiderTripsResponse>> GetRidersTripAsync(int riderId, string language);
        Task<bool> IsRiderQTripExistAsync(int riderqId);
        IList<TripsRiders> GetAll();
    }
}
