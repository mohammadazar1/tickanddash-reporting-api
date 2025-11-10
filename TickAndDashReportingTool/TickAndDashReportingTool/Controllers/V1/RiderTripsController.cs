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
    //[Authorize]
    public class RiderTripsController : ControllerBase
    {
        private IRiderTripsService _riderTripsService;

        public RiderTripsController(IRiderTripsService riderTripsService)
        {
            _riderTripsService = riderTripsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IActionResult actionResult = NotFound();

            IList<TripsRiders> carsQueue = _riderTripsService.GetAll();
            if (carsQueue.Count > 0)
                actionResult = Ok(carsQueue);

            return actionResult;
        }
    }
}
