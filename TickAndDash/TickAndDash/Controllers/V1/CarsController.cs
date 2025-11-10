 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using static TickAndDash.Filters.AuthorizationFilter;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ICarsQueueService _carsQueueService;
        private readonly IStringLocalizer<CarsController> _localizer;
        public CarsController( ICarService carService,
            ICarsQueueService carsQueueService, IStringLocalizer<CarsController> localizer)
        {
            _carService = carService;
            _carsQueueService = carsQueueService;
            _localizer = localizer;
        }

        /// <summary> 
        /// Request for the driver to login and logot form the car
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Only one driver can log into the car no driver can log into the car until that driver log out from it
        /// <br/> **Codes**  <br/>
        /// 1.Active_1 // There is an active driver on the car <br/>
        /// 2.sql_1 // database failure <br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "401"> UnAuthorized </response>
        /// <response code = "422"> Failed </response>
        /// <response code = "403"> Failed </response>
        [HttpPut("Toggle/online")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> LoginToCar()
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Code = Generalcodes.Gen_1.ToString(),
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int driverId);

            int carDriverId = await _carService.GetCarActiveDriverIdByCarIdAsync(carId);
            bool isUpdated;

            // No driver currently logged in to the car
            if (carDriverId == 0)
            {
                isUpdated = await _carService.UpdateLoggedInCarUserAsync(carId, driverId);
                if (!isUpdated)
                    return StatusCode(422, response);
            }
            
            // this driver is currently online over the car
            else if (carDriverId == driverId)
            {
                var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

                if (carsQueue != null)
                {
                    if (carsQueue.Turn == 1)
                    {
                        response.Code = QueusCode.car_4.ToString();
                        response.Message = _localizer["DriverTurn"].Value;/*"عذرًا، هنالك سائق فعال على السيارة حاليًا!";*/
                        return UnprocessableEntity(response);
                    }
                }

                isUpdated = await _carService.UpdateLoggedInCarUserAsync(carId, 0);
                if (!isUpdated)
                {
                    return StatusCode(422, response);
                }
                // Canceled due switching offline
                bool isCanceled = await _carsQueueService.CancelDriverCarReservationIfAnyAsync(carId);

                if (!isCanceled)
                {
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    response.Message = _localizer["GeneralMsg"].Value;/*TickAndDash.Language.Resource.GeneralErrorMsg;*/
                    return UnprocessableEntity(response);
                }
            }
            else
            {
                response.Code = ValidationCodes.Active_1.ToString();
                response.Message = _localizer["ActiveDriverError"].Value;/*"عذرًا، هنالك سائق فعال على السيارة حاليًا!";*/
                return UnprocessableEntity(response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; // "تم معالجة طلبك بنجاح";
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary> 
        /// Request for the driver to Get his Online/offline car login status 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **Returns Active or inActive status** <br/>
        /// **Codes** <br/>
        /// 
        /// 
        /// 1.Active_1 //There is an active driver on the car <br/>
        /// 2.sql_1 //database failure <br/>
        /// 3.Ok //success <br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        [HttpGet("Driver/Status")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<GetDriverStatusResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetDriverStatusOverTheCar()
        {
            GeneralResponse<GetDriverStatusResponse> response = new GeneralResponse<GetDriverStatusResponse>()
            {
                Message = _localizer["GeneralMsg"].Value,
                Data = new GetDriverStatusResponse(),
                Code = Generalcodes.Gen_1.ToString()
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int driverId);

            int carDriverId = await _carService.GetCarActiveDriverIdByCarIdAsync(carId);

            if (driverId == carDriverId)
            {
                response.Data.Status = "Active";
            }
            else
            {
                response.Data.Status = "Inactive";
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;/* "تم معالجة طلبك بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            return Ok(response);
        }

    }
}
