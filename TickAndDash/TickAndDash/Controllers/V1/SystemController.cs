using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Models;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Rider, Driver")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class SystemController : ControllerBase
    {

        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IStringLocalizer<SystemController> _localizer;

        public SystemController(ISystemConfigurationService systemConfigurationService, IStringLocalizer<SystemController> localizer)
        {
            _systemConfigurationService = systemConfigurationService;
            _localizer = localizer;
        }



        /// <summary>
        /// Reuqest for driver or rider to get current system configuration  
        /// </summary>
        /// <returns></returns>
        /// <response code = "200"> Success </response>
        /// <response code = "404"> Failed </response>
        [HttpGet("Configurations")]
        [Authorize]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetSystemConfig()
        {
            GeneralResponse<List<SystemConfiguration>> response = new GeneralResponse<List<SystemConfiguration>>()
            {
                Data = null
            };

            var systemSettings = await _systemConfigurationService.GetAllSystemConfigAsync();

            if (systemSettings == null || !systemSettings.Any())
            {
                response.Message = _localizer["NoInfoMsg"].Value;  /* "المعلومات غير متوفرة";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            response.Data = systemSettings;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }



    }
}
