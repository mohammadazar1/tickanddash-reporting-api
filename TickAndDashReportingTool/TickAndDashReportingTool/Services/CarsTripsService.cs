using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class CarsTripsService : ICarsTripsService
    {
        private readonly ICarsTripsDAL _carsTripsDAL;
        private readonly ITripsRidersDAL _tripsRidersDAL;

        public CarsTripsService(ICarsTripsDAL carsTripsDAL, ITripsRidersDAL tripsRidersDAL)
        {
            _carsTripsDAL = carsTripsDAL;
            _tripsRidersDAL = tripsRidersDAL;
        }

        public IList<CarsTrips> GetAll()
        {
            return _carsTripsDAL.GetAll();
        }

        public async Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId)
        {
            return await _tripsRidersDAL.AddToTripRiderAsync(riderQId, riderId, tripId);
        }

        public async Task<int> GetCarTripByCarsQueueIDAsync(int carqId)
        {
            return await _carsTripsDAL.GetCarTripByCarsQueueIDAsync(carqId);
        }
    }
}
