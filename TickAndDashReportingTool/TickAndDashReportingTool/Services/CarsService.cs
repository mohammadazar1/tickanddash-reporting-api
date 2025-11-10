using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class CarsService : ICarsService
    {
        private readonly ICarsDAL _carsDAL;

        public CarsService(ICarsDAL carsDAL)
        {
            _carsDAL = carsDAL;
        }

        public bool CreateCar(CreateCarRequest createCarRequest)
        {
            try
            {

                return _carsDAL.Create(new Car
                {
                    IsActive = true,
                    ItineraryId = createCarRequest.ItineraryId,
                    RegistrationPlate = createCarRequest.RegistrationPlate,
                    Model = createCarRequest.Model,
                    ModelYear = createCarRequest.ModelYear,
                    LoggedInDriverId = createCarRequest.LoggedInDriverId,
                    SeatCount = createCarRequest.SeatCount,
                    CarCode = createCarRequest.CarCode
                });
            }
            catch (SqlException ex)
            {
                throw new HttpStatusException(HttpStatusCode.BadRequest, "Nani?");
            }
        }


        public List<Car> GetAllCars()
        {
            var cars = _carsDAL.GetAll();
            if (cars.Count <= 0) throw new HttpStatusException(HttpStatusCode.NotFound, "No cars are available");
            return cars;
        }

        public bool UpdateCar(UpdateCarRequest putCarRequest)
        {
            if (_carsDAL.Update(new Car
            {
                Id = putCarRequest.Id,
                IsActive = putCarRequest.IsActive,
                ItineraryId = putCarRequest.ItineraryId,
                RegistrationPlate = putCarRequest.RegistrationPlate,
                Model = putCarRequest.Model,
                LoggedInDriverId = putCarRequest.LoggedInDriverId,
                SeatCount = putCarRequest.SeatCount,
            }))
                return true;

            throw new HttpStatusException(HttpStatusCode.BadRequest, "Nani?");
        }
    }
}
