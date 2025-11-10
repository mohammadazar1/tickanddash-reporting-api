using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class QueuesService : IQueuesService
    {
        private readonly ICarsQueueDAL _carsQueueDAL;
        private readonly IRidersQueueDAL _ridersQueueDAL;

        public QueuesService(ICarsQueueDAL carsQueueDAL, IRidersQueueDAL ridersQueueDAL)
        {
            _carsQueueDAL = carsQueueDAL;
            _ridersQueueDAL = ridersQueueDAL;
        }

        public async Task<int> GetRaiderDailyCancellationCountAsync(int riderId)
        {
            return await _ridersQueueDAL.GetRaiderDailyCancellationCountAsync(riderId);
        }

        //public bool AddCarToTheQueue(int carId, int pickupId)
        //{
        //    int turn = _carsQueueDAL.GetQLastCarTurn(pickupId);

        //    CarsQueue carsQueue = new CarsQueue()
        //    {
        //        CarId = carId,
        //        PickupStationId = pickupId,
        //        CreationDate = DateTime.Now,
        //        CarsQStatusLookupId = (int)CarsQStatusLookupEnum.InQueue,
        //        Turn = turn + 1
        //    };

        //    return _carsQueueDAL.AddCarToTheQueue(carsQueue);
        //}

        //public bool AddToRidersQueue(int riderId, int pickupId, RidersQStatusLookupEnum ridersQStatusLookupEnum, DateTime reservationDate, int CountOfSeats)
        //{
        //    RidersQueue ridersQueue = new RidersQueue()
        //    {
        //        RiderId = riderId,
        //        PickupStationId = pickupId,
        //        CreationDate = DateTime.Now,
        //        RidersQStatusLookupId = (int)ridersQStatusLookupEnum,
        //        ReservationDate = reservationDate,
        //        CountOfSeats = CountOfSeats,
        //        IsInQueue = true
        //    };

        //    return _ridersQueueDAL.AddToRidersQueue(ridersQueue);
        //}

        //public bool IsCarInQueue(int carId)
        //{
        //    return _carsQueueDAL.IsCarInQueue(carId);
        //}

        //public bool IsCarTurnActiveInTheStation(int carId, int psId)
        //{
        //    return _carsQueueDAL.IsCarTurnActiveInTheStation(carId, psId);
        //}

        //public bool UpdateCarSkipCount(int carId, int skipCount)
        //{
        //    return _carsQueueDAL.UpdateCarSkipCount(carId, skipCount);
        //}

        //public bool UpdateCarStatusInQueue(int carId, int psId, CarsQStatusLookupEnum status)
        //{
        //    return _carsQueueDAL.UpdateCarStatusInQueue(carId, psId, status);
        //}

        //public bool UpdateRiderStatusInQueue(int riderId, RidersQStatusLookupEnum status)
        //{
        //    bool isUpdated = _ridersQueueDAL.UpdateRiderStatusInQueue(riderId, status);
        //    //if (isUpdated)
        //    //{
        //    //    return UpdateRiderIsInQueueStatus(riderId, false);
        //    //}

        //    return isUpdated;
        //}



        //public bool IsRiderInQueueWithStatus(int riderId, RidersQStatusLookupEnum status)
        //{
        //    return _ridersQueueDAL.IsRiderInQueueWithStatus(riderId, status);
        //}

        //public bool UpdateRiderSkipCount(int riderId, int skipCount)
        //{
        //    return _ridersQueueDAL.UpdateRiderSkipCount(riderId, skipCount);
        //}

        //public int GetCountOfCarsInPickupStation(int psId)
        //{
        //    return _carsQueueDAL.GetCountOfCarsInPickupStation(psId);
        //}

        public List<RidersQueue> GetAllRidersInQ(RidersQStatusLookupEnum status, int ps)
        {

            return _ridersQueueDAL.GetAllRidersInQ(status, ps);
        }

        public async Task<RidersQueue> GetRidersInQAsync(int riderQId, RidersQStatusLookupEnum status)
        {
            return await _ridersQueueDAL.GetRidersInQAsync(riderQId, status);
        }

        //public CarsQueue GetActiveCarInCarQ(int carId)
        //{
        //    return _carsQueueDAL.GetActiveCarInCarQ(carId);
        //}

        //public CarsQueue GetCarTurnInPPickup(int pickUpId)
        //{
        //    return _carsQueueDAL.GetCarTurnInPickup(pickUpId);
        //}

        //public RidersQueue GetRidersInQbyUserId(int userId, RidersQStatusLookupEnum status)
        //{
        //    return _ridersQueueDAL.GetRidersInQbyUserId(userId, status);

        //}

        //public bool CancelAllExpiredReservation(int riderId)
        //{

        //    return _ridersQueueDAL.CancelAllExpiredReservation(riderId);
        //}

        public int GetCountOfRidersInFrontOfMyTurn(DateTime reservationdatetime, int psId)
        {
            return _ridersQueueDAL.GetCountOfRidersInFrontOfMyTurn(reservationdatetime, psId);
        }



        //public Driver GetActiveDriverInPickupQ(int pickupId)
        //{
        //    return _carsQueueDAL.GetActiveDriverInPickupQ(pickupId);
        //}

        public int GetPickupStationsIdForRiderInQueueWithStatus(int riderId, RidersQStatusLookupEnum status)
        {
            return _ridersQueueDAL.GetPickupStationsIdForRiderInQueueWithStatus(riderId, status);
        }

        //public List<Driver> GetDriversInPickupQ(int pickupId, CarsQStatusLookupEnum status)
        //{
        //    return _carsQueueDAL.GetDriversInPickupQ(pickupId, status);
        //}

        //public int GetCountOfCarsInFrontOfMyTurn(int carId, int psId)
        //{
        //    return _carsQueueDAL.GetCountOfCarsInFrontOfMyTurn(carId, psId);
        //}

        //public int GetCountOfCarsAfterMyTurn(int carId, int psId)
        //{
        //    return _carsQueueDAL.GetCountOfCarsAfterMyTurn(carId, psId);
        //}


        //public int GetCountOfCarsInFrontOfMyTurn(int carId)
        //{
        //    return _carsQueueDAL.GetCountOfCarsInFrontOfMyTurn(carId);
        //}

        public bool CancelRiderReservation(int riderId)
        {
            return _ridersQueueDAL.CancelRiderReservation(riderId);

        }

        public bool UpdateRiderIsInQueueStatus(int riderId, bool isInQueue)
        {
            return _ridersQueueDAL.UpdateRiderIsInQueueStatus(riderId, isInQueue);

        }

        public bool DoesRiderHasAnActiveReservationInTheQueue(int riderId)
        {
            return false;
            //return _ridersQueueDAL.UpdateRiderIsInQueueStatus(riderId,);

        }

        //public List<RidersQueue> GetAllWaitingRidersInPickupStationQ(int ps)
        //{
        //    int queueReservationtimeLimit = 30;
        //    int notificationMinutesToExpire = 5;

        //    return _ridersQueueDAL.GetAllWaitingRidersInPickupStationQ(queueReservationtimeLimit, notificationMinutesToExpire, ps);
        //}

        public bool UpdateRiderStatusInQueueByRiderqId(int riderQId, RidersQStatusLookupEnum status)
        {
            return _ridersQueueDAL.UpdateRiderStatusInQueueByRiderqId(riderQId, status);
        }

        //public async Task<RidersQueue> GetRidersInQ(int riderQId)
        //{
        //    return await _ridersQueueDAL.GetRidersInQAsync(riderQId);
        //}

        public async Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId)
        {
            return await _ridersQueueDAL.GetRaiderWeeklyCancellationCountAsync(riderId);
        }




        //public List<RidersQueue> GetAllActiveRidersReservationInPickupStation(int psId)
        //{
        //    int notificationMinutesToExpire = 5;
        //    return _ridersQueueDAL.GetAllActiveRidersReservationInPickupStation(psId, notificationMinutesToExpire);
        //}

        //public RidersQueue GetRiderActiveReservation(int riderId)
        //{
        //    return _ridersQueueDAL.GetRiderActiveReservation(riderId);
        //}

        //public int GetCountOfCarsInQueue(int psId)
        //{
        //    return _carsQueueDAL.GetCountOfCarsInQueue(psId);

        //}


        //public bool CancelDriverCarReservationIfAny(int carId)
        //{
        //    bool success = true;

        //    var carsQueue = _carsQueueDAL.GetActiveCarInCarQ(carId);

        //    if (carsQueue != null)
        //    {
        //        success = _carsQueueDAL.UpdateCarStatusInQueue(carId, carsQueue.PickupStationId, CarsQStatusLookupEnum.Cancelled);
        //    }

        //    return success;
        //}

        ////public bool UpdateCarsSkipCount(int carqId, int psId)
        ////{
        ////    return _carsQueueDAL.UpdateCarsSkipCount(carqId, psId);
        ////}

        //public CarsQueue GetQCurrentCarsQueueTurn(int pickupId)
        //{
        //    return _carsQueueDAL.GetQCurrentCarsQueueTurn(pickupId);
        //}

        //public int GetQLastCarTurn(int pickupId)
        //{
        //    return _carsQueueDAL.GetQLastCarTurn(pickupId);
        //}

        //public int GetCarQTurn(int pickupId, int carId)
        //{
        //    return _carsQueueDAL.GetCarQTurn(pickupId, carId);
        //}

        //public bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int FromTurn, int ToTurn)
        //{
        //   return _carsQueueDAL.UpdateCarsTurnAfterSkip(pickupId, carId, FromTurn, ToTurn);
        //}

        //public bool UpdateCarsTurnAfterCancelation(int pickupId, int turn)
        //{
        //    return _carsQueueDAL.UpdateCarsTurnAfterCancelation(pickupId, turn);
        //}

    }
}
