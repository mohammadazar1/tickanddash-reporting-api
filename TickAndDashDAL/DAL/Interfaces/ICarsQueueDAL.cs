using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface ICarsQueueDAL
    {
        IList<CarsQueue> GetAllForTodayByItineraryId(int itineraryId, string lang);
        Task<CarsQueue> GetActiveCarInCarQAsync(int carId);
        Task<Driver> GetActiveDriverInPickupQAsync(int psId);
        Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId);
        Task<List<CarsQueue>> GetDriversInPickupQAsync(int pickupId, CarsQStatusLookupEnum status);
        Task<CarsQueue> GetCurrentActiveCarsTurnInPickupStationWithSkippedAsync(int pickupId);
        Task<CarsQueue> GetCarQueueByTurnAsync(int pickupId, int turn);
        Task<int> GetCountOfCarsBeforeMyTurnAsync(int carId, int psId);
        Task<int> GetCountOfCarsAfterMyTurnAsync(int carId, int psId);
        Task<int> GetCountOfCarsInQueueAsync(int psId);
        Task<int> GetQLastCarTurnAsync(int pickupId);
        Task<int> GetCarQTurnAsync(int pickupId, int carId);
        Task<int> AddCarToTheQueueAsync(CarsQueue carsQueue);
        Task<bool> UpdateCarSkipCountAsync(int carId, int skipCount);
        Task<bool> UpdateCarStatusInQueueAsync(int carId, int psId, CarsQStatusLookupEnum status);
        bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int FromTurn, int ToTurn);
        bool UpdateIsCarNotifiedAboutTurnInPickupStation(int carqId, bool isNoti);
        bool UpdateCarsTurnAfterCancelation(int pickupId, int turn);
        bool Update(CarsQueue carsQueue);

        Task<bool> IsCarTurnActiveInTheStationAsync(int carId, int psId);
        Task<bool> IsCarInQueueAsync(int carId);
        Task<DateTime?> GetTheLastTimeCarLeftTheQ(int carId, int psId);
        //bool CancelDriverCarReservation(int carId, int psId);
        //int GetCountOfCarsInPickupStation(int psId);
        //CarsQueue GetCarTurnInPickup(int pickupId);
        //bool UpdateCarsSkipCount(int carqId, int psId);
        //int GetCountOfCarsInFrontOfMyTurn(int carId);

    }

}
