using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class CarsQueueService : ICarsQueueService
    {
        private readonly ICarsQueueDAL _carsQueueDAL;
        private readonly ITripsService _tripsService;
        private readonly IPickupStationsService _pickupStationsService;

        public CarsQueueService(ICarsQueueDAL carsQueueDAL, ITripsService tripsService, IPickupStationsService pickupStationsService)
        {
            _carsQueueDAL = carsQueueDAL;
            _tripsService = tripsService;
            _pickupStationsService = pickupStationsService;
        }

        public async Task<bool> AddCarToTheQueueAsync(int carId, int pickupId, int driverId)
        {
            //int turn = await _carsQueueDAL.GetQLastCarTurnAsync(pickupId);

            CarsQueue carsQueue = new CarsQueue()
            {
                CarId = carId,
                PickupStationId = pickupId,
                CreationDate = DateTime.Now,
                CarsQStatusLookupId = (int)CarsQStatusLookupEnum.InQueue
                //Turn = turn + 1
            };

            int carQId = await _carsQueueDAL.AddCarToTheQueueAsync(carsQueue);

            if (carQId <= 0)
            {
                return false;
            }

            return await _tripsService.AddCarTripAsync( carId ,carQId, driverId);
        }

        public async Task<bool> CancelDriverCarReservationIfAnyAsync(int carId)
        {
            bool success = true;

            var carsQueue = await _carsQueueDAL.GetActiveCarInCarQAsync(carId);

            if (carsQueue != null)
            {
                success = await _carsQueueDAL.UpdateCarStatusInQueueAsync(carId, carsQueue.PickupStationId, CarsQStatusLookupEnum.offlineCancellation);
            }

            return success;
        }

        public async Task<CarsQueue> GetActiveCarInCarQAsync(int carId)
        {
            return await _carsQueueDAL.GetActiveCarInCarQAsync(carId);
        }

        public async Task<Driver> GetActiveDriverInPickupQAsync(int pickupId)
        {
            int psId = await _pickupStationsService.GetMinorPickupStationMainStaionId(pickupId);

            pickupId = psId == 0 ? pickupId : psId;

            return await _carsQueueDAL.GetActiveDriverInPickupQAsync(pickupId);
        }

        public async Task<int> GetCarQTurnAsync(int pickupId, int carId)
        {
            return await _carsQueueDAL.GetCarQTurnAsync(pickupId, carId);
        }

        public async Task<int> GetCountOfCarsAfterMyTurnAsync(int carId, int psId)
        {
            return await _carsQueueDAL.GetCountOfCarsAfterMyTurnAsync(carId, psId);
        }

        public async Task<int> GetCountOfCarsBeforeMyTurnAsync(int carId, int psId)
        {
            return await _carsQueueDAL.GetCountOfCarsBeforeMyTurnAsync(carId, psId);
        }

        public async Task<int> GetCountOfCarsInQueueAsync(int psId)
        {
            return await _carsQueueDAL.GetCountOfCarsInQueueAsync(psId);
        }

        public async Task<List<CarsQueue>> GetDriversInPickupQAsync(int pickupId, CarsQStatusLookupEnum status)
        {
            return await _carsQueueDAL.GetDriversInPickupQAsync(pickupId, status);
        }

        public async Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId)
        {
            int psId = await _pickupStationsService.GetMinorPickupStationMainStaionId(pickupId);
            pickupId = psId == 0 ? pickupId : psId;

            return await _carsQueueDAL.GetQCurrentCarsQueueTurnAsync(pickupId);
        }

        public async Task<int> GetQLastCarTurnAsync(int pickupId)
        {
            return await _carsQueueDAL.GetQLastCarTurnAsync(pickupId);
        }

        public async Task<bool> IsCarInQueueAsync(int carId)
        {
            return await _carsQueueDAL.IsCarInQueueAsync(carId);
        }

        public async Task<bool> IsCarTimeLimitationValidToEnterTheQ(int carId, int psId, int timeLimit)
        {
            bool isCarLimitTimeValid = true;
            var lastdatetime = await _carsQueueDAL.GetTheLastTimeCarLeftTheQ(carId, psId);

            if (lastdatetime.HasValue && lastdatetime != null && lastdatetime != default(DateTime))
            {
                if ((DateTime.Now - lastdatetime.Value).TotalMinutes < timeLimit)
                {
                    isCarLimitTimeValid = false;
                }
            }

            return isCarLimitTimeValid;
        }

        public async Task<bool> IsCarTurnActiveInTheStationAsync(int carId, int psId)
        {
            return await _carsQueueDAL.IsCarTurnActiveInTheStationAsync(carId, psId);
        }

        public async Task<bool> UpdateCarSkipCountAsync(int carId, int skipCount)
        {
            return await _carsQueueDAL.UpdateCarSkipCountAsync(carId, skipCount);
        }

        public async Task<bool> UpdateCarStatusInQueueAsync(int carId, int psId, CarsQStatusLookupEnum status)
        {
            return await _carsQueueDAL.UpdateCarStatusInQueueAsync(carId, psId, status);
        }

        public bool UpdateCarsTurnAfterCancelation(int pickupId, int turn)
        {
            return _carsQueueDAL.UpdateCarsTurnAfterCancelation(pickupId, turn);
        }

        public bool UpdateCarsTurnAfterSkip(int pickupId, int carId, int FromTurn, int ToTurn)
        {
            return _carsQueueDAL.UpdateCarsTurnAfterSkip(pickupId, carId, FromTurn, ToTurn);
        }
    }
}
