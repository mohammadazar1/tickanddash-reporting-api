using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class CarsQueuesController : ControllerBase
    {
        private readonly ICarsQueuesService _carsQueuesService;

        public CarsQueuesController(ICarsQueuesService carsQueuesService)
        {
            _carsQueuesService = carsQueuesService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] GetCarsQueueRequest getCarsQueueRequest)
        {
            IActionResult actionResult = NotFound();

            IList<CarsQueue> carsQueue = _carsQueuesService.GetAll(getCarsQueueRequest);
            if (carsQueue != null && carsQueue.Count > 0)
                actionResult = Ok(carsQueue);

            return actionResult;
        }

        [HttpPut]
        public IActionResult Put([FromBody] UpdateCarsQueueRequest updateCarsQueueRequest)
        {
            IActionResult actionRuslt = BadRequest();

            if (_carsQueuesService.Update(updateCarsQueueRequest))
                actionRuslt = Ok();

            return actionRuslt;
        }

        [HttpPost("force-go")]
        public async Task<ActionResult> ForceGo([FromBody] ForceGoRequest forceGoRequest)
        {
            bool result = await _carsQueuesService.ForceGo(forceGoRequest);

            if (result)
                return Ok();

            return BadRequest();
        }
    }
}
