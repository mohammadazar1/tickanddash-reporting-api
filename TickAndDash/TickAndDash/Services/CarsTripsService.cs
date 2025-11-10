using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class CarsTripsService : ICarsTripsService
    {

        private readonly ICarsTripsDAL _carsTripsDAL;
        private readonly IActionContextAccessor _actionContextAccessor;

        public CarsTripsService(ICarsTripsDAL carsTripsDAL, IActionContextAccessor actionContextAccessor)
        {
            _carsTripsDAL = carsTripsDAL;
            _actionContextAccessor = actionContextAccessor;

        }

        public async Task<List<RidersTickets>> GetLastCarTripInfoAsync(int carId)
        {
            string _language = _actionContextAccessor.ActionContext.HttpContext.Request.Headers["Content-Language"];

            return await _carsTripsDAL.GetLastCarTripInfoAsync(carId, _language);
        }

        public async Task<int> GetRiderQIdFromLastCarTripAsync(string carCode, int riderId)
        {
            return await _carsTripsDAL.GetRiderQIdFromLastCarTripAsync(carCode, riderId);
        }
    }
}
