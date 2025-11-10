using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using static TickAndDashDAL.DAL.TripsRidersDAL;

namespace TickAndDash.Services
{
    public class TripsService : ITripsService
    {
        public readonly ICarsTripsDAL _carsTripsDAL;
        public readonly ITripsRidersDAL _tripsRidersDAL;
        private readonly IActionContextAccessor _actionContextAccessor;

        public TripsService(ICarsTripsDAL carsTripsDAL, ITripsRidersDAL tripsRidersDAL, IActionContextAccessor actionContextAccessor)
        {
            _carsTripsDAL = carsTripsDAL;
            _tripsRidersDAL = tripsRidersDAL;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<bool> AddToTripRiderAsync(int riderQId, int riderId, int tripId)
        {
            return await _tripsRidersDAL.AddToTripRiderAsync(riderQId, riderId, tripId);
        }

        public async Task<int> GetCarTripByCarsQueueIDAsync(int carqId)
        {
            return await _carsTripsDAL.GetCarTripByCarsQueueIDAsync(carqId);
        }

        public async Task<List<GetRiderTripsResponse>> GetRidersTripAsync(int riderId)
        {
            string language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];
            return await _tripsRidersDAL.GetRidersTripAsync(riderId, language);
        }

        public async Task<bool> AddCarTripAsync(int carId, int carqId, int DriverId)
        {
            bool success;

            bool isTripExisted = await _carsTripsDAL.IsCarTripExistAsync(carqId);

            if (!isTripExisted)
            {
                CarsTrips carsTrips = new CarsTrips()
                {
                    CarsQueueId = carqId,
                    CreationDate = DateTime.Now,
                    DriverId = DriverId,// carq.Car.LoggedInDriver.UserId
                    CarId = carId
                };

                success = await _carsTripsDAL.InsertCarTripAsync(carsTrips);
            }
            else
            {
                success = true;
            }

            return success;
        }

        public async Task<bool> IsRiderQTripExistAsync(int riderqId)
        {
            return await _tripsRidersDAL.IsRiderQTripExistAsync(riderqId);
        }

        //public List<TripsRiders> GetRidersTrip(int riderId)
        //{
        //    return _tripsRidersDAL.GetRidersTrip(riderId);

        //}



    }
}
