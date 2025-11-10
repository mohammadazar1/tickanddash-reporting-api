using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ICarsTripsDAL
    {

        Task<bool >InsertCarTripAsync(CarsTrips carsTrip);

        Task<bool >IsCarTripExistAsync(int carqId);

        Task<int> GetCarTripByCarsQueueIDAsync(int carqId);
        IList<CarsTrips> GetAll();
        Task<Driver> GetDriverByCarQIdAsync(int id);
        Task<int> GetRiderQIdFromLastCarTripAsync(string carId, int riderId);

        Task<List<RidersTickets>> GetLastCarTripInfoAsync(int carId, string lang);


    }
}
