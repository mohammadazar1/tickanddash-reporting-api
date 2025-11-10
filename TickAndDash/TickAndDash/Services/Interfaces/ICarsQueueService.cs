using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface ICarsQueueService
    {
        Task<bool> IsCarTimeLimitationValidToEnterTheQ(int carId, int psId, int timeLimt);
        Task<CarsQueue> GetActiveCarInCarQAsync(int carId);
        Task<Driver> GetActiveDriverInPickupQAsync(int pickupId);
        Task<List<CarsQueue>> GetDriversInPickupQAsync(int pickupId, CarsQStatusLookupEnum status);
        Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId);
        Task<int> GetQLastCarTurnAsync (int pickupId);
        Task<int> GetCarQTurnAsync(int pickupId, int carId);
        Task<int> GetCountOfCarsBeforeMyTurnAsync(int carId, int psId);
        Task<int> GetCountOfCarsAfterMyTurnAsync(int carId, int psId);
        Task<int> GetCountOfCarsInQueueAsync(int psId);


        Task<bool> AddCarToTheQueueAsync(int carId, int pickupId, int driverId);
        Task<bool> UpdateCarSkipCountAsync(int carId, int skipCount);
        Task<bool> UpdateCarStatusInQueueAsync(int carId, int psId, CarsQStatusLookupEnum status);
        bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int FromTurn, int ToTurn);
        bool UpdateCarsTurnAfterCancelation(int pickupId, int turn);

        Task<bool> IsCarTurnActiveInTheStationAsync(int carId, int psId);
        Task<bool> IsCarInQueueAsync(int carId);

        Task<bool> CancelDriverCarReservationIfAnyAsync(int carId);


    }
}
