using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Extensions;
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class SMSController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IStringLocalizer<SMSController> _localizer;
        private readonly ISMSService _sMSService;
        private readonly IBulkSMSService _bulkSMSService;
        private readonly ILogger<SMSController> _logger;
        private readonly ISystemConfigurationService _ISystemConfigurationService;
        private readonly IUsersService _usersService;

        public SMSController(IUsersService userService, ISMSService sMSService,
            IStringLocalizer<SMSController> localizer, IBulkSMSService bulkSMSService,
            ILogger<SMSController> logger,
            ISystemConfigurationService SystemConfigurationService,
             IUsersService usersService
            )
        {
            _sMSService = sMSService;
            _userService = userService;
            _localizer = localizer;
            _bulkSMSService = bulkSMSService;
            _logger = logger;
            _ISystemConfigurationService = SystemConfigurationService;
            _usersService = usersService;
        }


        /// <summary>
        /// Rider request to resend pin code if it is not delivered for any reason
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.MobVal_1 Mobile Number is not valid<br/>
        /// 2.Ok Success<br/>
        /// 3.Pin_2 pincode Expired<br/>
        /// 4.Pin_1 wrong pin code<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> input validation error </response>
        /// <response code = "404"> Not Found User </response>
        /// <response code = "422"> Unable to resent the pincode for any reason </response>
        [HttpPut("Pincode/Resent")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> ReSentPinCode([FromBody] ResentPincodeRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null,
                Code = ""
            };

            bool isValidMobileNumber = request.MobileNumber.IsPalMobileNumber();
            if (!isValidMobileNumber)
            {
                response.Message = _localizer["InvalidMSISDN"].Value;

                response.Code = ValidationCodes.MobVal_1.ToString();
                Helpers.LogReponse(response, "ResentPincode", "Resent SMS Pincode  Response");
                return BadRequest(response);
            }

            if (int.TryParse(await _ISystemConfigurationService.
             GetSettingValueByKeyAsync(SettingKeyEnum.PinCodesLimit), out int pinCodesLimit))
            {
                int countOfPins = await _usersService.GetCountOfPinCodesGeneratedAsync(request.MobileNumber);
                if (countOfPins >= pinCodesLimit)
                {

                    response.Message = _localizer["PincodesLimits"].Value;
                    response.Code = ValidationCodes.Pin_2.ToString();
                    return BadRequest(response);
                }
            }


            request.MobileNumber = request.MobileNumber.ConvertMobileNumberFormate();

            var rider = await _userService.GetRiderByMobileNumberAsync(request.MobileNumber);
            if (rider?.User == null)
            {
                response.Message = _localizer["UnregisteredAccountMsg"].Value;  /* "الحساب غير معرف، يرجى المحاولة مجددًا";*/
                response.Code = UserCodes.Usr_2.ToString();
                Helpers.LogReponse(response, "ResentPincode", "Resent SMS Pincode  Response");
                return NotFound(response);
            }

            do
            {
                rider.GeneratedPincode = Helpers.GenerateRandomPinCode();
            } while (rider.LoginPincode == rider.GeneratedPincode);

            await _userService.UpdateRiderGeneratedPincodeAsync(rider);

            bool success = await _sMSService.SendSMSToUserAsync(rider.MobileNumber, rider.GeneratedPincode.ToString());
            //bool success = await _bulkSMSService.SendSMSAsync(new Services.ServicesDtos.SMSDto
            //{
            //    MSISDN = rider.MobileNumber,
            //    Msg = $"code :{rider.GeneratedPincode.ToString()}"
            //});

            //if (!success)
            //{
            //    //_logger.LogError($"{request.MobileNumber}: something went wrong while sending the pinccode");
            //    response.Message = _localizer["PincodeSentError"].Value; /*"حدث خطأ اثناء ارسال رمز التفعيل!، يرجى المحاولة لاحقًا";*/
            //    response.Code = UnExpectedErrors.Pin_3.ToString();
            //    Helpers.LogReponse(response, "ResentPincode", "Resent SMS Pincode  Response");
            //    return StatusCode(422, response);
            //}

            response.Code = Generalcodes.Ok.ToString();
            response.Message = _localizer["SuccessMsg"].Value; /* "تم ارسال رمز التفعيل بنجاح";*/
            response.Success = true;
            Helpers.LogReponse(response, "ResentPincode", "Resent SMS Pincode  Response");

            return Ok(response);
        }

    }
}
