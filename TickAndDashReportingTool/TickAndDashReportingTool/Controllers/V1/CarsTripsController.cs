using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class CarsTripsController : ControllerBase
    {
        private readonly ICarsTripsService _carsTripsService;

        public CarsTripsController(ICarsTripsService carsTripsService)
        {
            _carsTripsService = carsTripsService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IActionResult actionResult = NotFound();

            IList<CarsTrips> carsQueue = _carsTripsService.GetAll();
            if (carsQueue.Count > 0)
                actionResult = Ok(carsQueue);

            return actionResult;
        }
    }
}
