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
    public class RidersQueuesController : ControllerBase
    {
        private readonly IRiderQueueService _riderQueueService;

        public RidersQueuesController(IRiderQueueService riderQueueService)
        {
            _riderQueueService = riderQueueService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int psId)
        {
            List<RidersQueue> ridersQueues = _riderQueueService.GetAll(psId); 
            return Ok(ridersQueues);
        }
    }
}
