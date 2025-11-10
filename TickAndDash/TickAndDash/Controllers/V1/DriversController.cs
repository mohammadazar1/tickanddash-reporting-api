using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services.Interfaces;
using static TickAndDash.Filters.AuthorizationFilter;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class DriversController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly IStringLocalizer<DriversController> _localizer;

        public DriversController(ICarService carService, IStringLocalizer<DriversController> localizer)
        {
            _carService = carService;
            _localizer = localizer;
        }


        /// <summary>
        /// Request for the driver to Get his configuration settings
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Returns all driver fixed info like: car seat number
        /// **Codes** 
        /// 1. Active_1 car is not active <br/>
        /// 2. Ok //success <br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response>
        [HttpGet("Configuration")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<GetDriverConfigResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetDriverConfig()
        {
            GeneralResponse<GetDriverConfigResponse> response = new GeneralResponse<GetDriverConfigResponse>()
            {
                Data = null,
                Code = ""
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            bool isCarActive = await _carService.IsCarActiveAsync(carId);

            if (!isCarActive)
            {
                response.Message = _localizer["InActiveCar"].Value;  /* "عذرًا، السيارة غير مفعلة حاليًا";*/
                response.Code = ValidationCodes.Active_1.ToString();
                return BadRequest(response);
            }

            int countOfSeats = await _carService.GetCarCountOfSeatsAsync(carId);

            response.Data = new GetDriverConfigResponse();
            response.Data.SeatsCount = countOfSeats;

            response.Data.Language = Request.Headers["Content-Language"];
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
