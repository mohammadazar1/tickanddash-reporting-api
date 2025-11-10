using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IQueuesService
    {
        // Car
        //bool AddCarToTheQueue(int carId, int pickupId);
        //bool IsCarTurnActiveInTheStation(int carId, int psId);
        //bool IsCarInQueue(int carId);
        //bool UpdateCarSkipCount(int carId, int skipCount);
        ////bool UpdateCarsSkipCount(int carqId, int psId);
        //bool UpdateCarStatusInQueue(int carId, int psId, CarsQStatusLookupEnum status);
        ////int GetCountOfCarsInPickupStation(int psId);
        //CarsQueue GetActiveCarInCarQ(int carId);
        ////CarsQueue GetCarTurnInPPickup(int pickupId);
        //Driver GetActiveDriverInPickupQ(int pickupId);
        //List<Driver> GetDriversInPickupQ(int pickupId, CarsQStatusLookupEnum status);
        //int GetCountOfCarsInFrontOfMyTurn(int carId, int psId);
        //int GetCountOfCarsAfterMyTurn(int carId, int psId);
        //bool CancelDriverCarReservationIfAny(int carId);
        //int GetCountOfCarsInQueue(int psId);
        //CarsQueue GetQCurrentCarsQueueTurn(int pickupId);
        //int GetQLastCarTurn(int pickupId);
        //int GetCarQTurn(int pickupId, int carId);
        //bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int FromTurn, int ToTurn);
        //bool UpdateCarsTurnAfterCancelation(int pickupId, int turn);
        //int GetCountOfCarsInFrontOfMyTurn(int carId);
        // Rider
        //bool AddToRidersQueue(int riderId, int pickupId, RidersQStatusLookupEnum ridersQStatusLookupEnum, DateTime date, int CountOfSeats);
        //bool UpdateRiderStatusInQueue(int riderId, RidersQStatusLookupEnum status);
        //bool CancelAllExpiredReservation(int riderId);

        bool UpdateRiderStatusInQueueByRiderqId(int riderQId, RidersQStatusLookupEnum status);
        bool UpdateRiderIsInQueueStatus(int riderId, bool isInQueue);
        bool CancelRiderReservation(int riderId);

        bool DoesRiderHasAnActiveReservationInTheQueue(int riderId);
        int GetPickupStationsIdForRiderInQueueWithStatus(int riderId, RidersQStatusLookupEnum status);
        
        //bool UpdateRiderSkipCount(int riderId, TimeSpan timeSpan);
        List<RidersQueue> GetAllRidersInQ(RidersQStatusLookupEnum status, int ps);
        //List<RidersQueue> GetAllWaitingRidersInPickupStationQ(int ps);
        //List<RidersQueue> GetAllActiveRidersReservationInPickupStation(int ps);
        //Task<RidersQueue> GetRidersInQAsync(int riderQId, RidersQStatusLookupEnum status);
        //RidersQueue GetRidersInQ(int riderQId);
        //RidersQueue GetRidersInQbyUserId(int riderQId, RidersQStatusLookupEnum status);
        //RidersQueue GetRiderActiveReservation(int riderId);
        int GetCountOfRidersInFrontOfMyTurn(DateTime reservationdatetime, int psId);

        Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId);
        Task<int> GetRaiderDailyCancellationCountAsync(int riderId);
    }
}
