using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Services.Interfaces;
using TickAndDashReportingTool.Controllers.V1.Requests;
using Microsoft.AspNetCore.Authorization;

namespace TickAndDashReportingTool.Controllers.V1
{
    [Route("api/report/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemConfigurationsController : ControllerBase
    {

        private readonly ISystemConfigurationService _systemConfigurationService;

        public SystemConfigurationsController(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService = systemConfigurationService;
        }


        [HttpGet]
        public async Task<ActionResult> Get()
        {
            List<SystemConfiguration> res = await _systemConfigurationService.GetAllConfigurationAsync();
            return Ok(res);
        }

        [HttpPut]
        public IActionResult Put([FromBody] UpdateSystemConfigRequest updateSystemConfigRequest)
        {
            bool result = _systemConfigurationService.Update(updateSystemConfigRequest);
            return Ok();
        }
    }
}
