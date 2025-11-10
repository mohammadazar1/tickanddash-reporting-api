using AutoMapper;
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
using static TickAndDash.Filters.AuthorizationFilter;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]

    public class ProfileController : ControllerBase
    {

        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<ProfileController> _localizer;



        public ProfileController(IUsersService usersService, IMapper mapper,
            IStringLocalizer<ProfileController> localizer
            )
        {
            _usersService = usersService;
            _mapper = mapper;
            _localizer = localizer;
        }


        /// <summary>
        ///  Request for Geting Rider Profile info
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Data_1, // No mathing data
        /// 2.Ok // Success
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response> 
        /// <response code = "404"> Failed </response> 
        [HttpGet("Rider")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [ProducesResponseType(typeof(GeneralResponse<RiderInfoResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetRiderInfo()
        {
            GeneralResponse<RiderInfoResponse> response = new GeneralResponse<RiderInfoResponse>()
            {
                Data = null,
                Code = ""
            };

            string mobileNumber = User?.FindFirstValue(ClaimsEnum.MobileNumber.ToString());

            var userInfo = await _usersService.GetRiderByMobileNumberAsync(mobileNumber);

            if (userInfo == null)
            {
                response.Message = _localizer["UnRegisteredAccount"].Value; /* "عذرًا، رقم الهاتف غير معرف";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var riderInfo = _mapper.Map<RiderInfoResponse>(userInfo);
            response.Data = riderInfo;

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        ///  Request for Geting Rider Profile info
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response>
        [HttpPut("Rider")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> UpdateRiderInfo([FromBody] UpdateRiderInfo request)
        {
            if (request is null)
            {
                request = new UpdateRiderInfo();
            }

            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            bool isUpdated = await _usersService.UpdateRiderInfoAsync(userId, request.Name, request.Gender);
            if (!isUpdated)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value;/* "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return StatusCode(422, response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        ///  Request for Geting Drivers Profile info
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response> 
        [HttpGet("Driver")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<DriverInfoResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetDriverProfileInfo()
        {
            GeneralResponse<DriverInfoResponse> response = new GeneralResponse<DriverInfoResponse>()
            {
                Data = new DriverInfoResponse(),
                Code = ""
            };

            string licenseNumber = User.FindFirstValue(ClaimsEnum.LicenseNumber.ToString());

            var drivers = await _usersService.GetDriverByLicenseNumberAsync(licenseNumber);
            if (drivers == null)
            {
                response.Message = _localizer["UnRegisteredAccount"].Value; /* "عذرًا، هذا الحساب غير معرف";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound();
            }

            var driverInfo = _mapper.Map<DriverInfoResponse>(drivers);
            response.Data = driverInfo;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }


        [HttpPut("Driver")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> UpdateDriverInfo([FromBody] UpdateDriverInfoRequest request)
        {
            if (request is null)
                return NotFound();

            GeneralResponse<RiderInfoResponse> response = new GeneralResponse<RiderInfoResponse>()
            {
                Data = null,
                Code = ""
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            if (!string.IsNullOrWhiteSpace(request.password))
            {
                bool isPasswordCorrect = await _usersService.IsDriverPasswordCorrectAsync(userId, request.oldPassword);

                if (!isPasswordCorrect)
                {
                    response.Message = _localizer["IncorrectPassword"].Value;/*"عذرًا، رمز المرور المدخل غير صحيح";*/
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    return StatusCode(400, response);
                }
            }

            var isUpdated = await _usersService.UpdateDriverInfoAsync(userId, request.Name, request.password);

            if (!isUpdated)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value;/* "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return StatusCode(422, response);
            }


            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;/*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        ///  Request for Updating Users Language (Ar/En Values)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 
        /// 
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response> 
        [HttpPost("Language")]
        [Authorize(Roles = "Driver, Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> UpdateLanguage([FromBody] UpdateUserLanguage updateUserLanguage)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null,
                Code = ""
            };

            if (updateUserLanguage?.Language?.ToLower() != "ar" && updateUserLanguage?.Language?.ToLower() != "en")
            {
                ModelState.AddModelError("Language", "ar/en are the valid value for language");
            }

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            bool isUpdated = await _usersService.UpdateUserLanguageAsync(userId, updateUserLanguage.Language);

            if (!isUpdated)
            {
                response.Code = UnExpectedErrors.sql_1.ToString();
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا!";*/
                return UnprocessableEntity(response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب ينجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return StatusCode(201, response);
        }

        /// <summary>
        /// Request for Drivers and Riders to get their setting language
        /// </summary>
        /// <returns></returns>

        [HttpGet("Language")]
        [Authorize(Roles = "Driver, Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetUserLanguage()
        {
            GeneralResponse<GetLanguageInfoResponse> response = new GeneralResponse<GetLanguageInfoResponse>()
            {
                Data = null,
                Code = ""
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            var user = await _usersService.GetUserAsync(userId, true);

            response.Data = new GetLanguageInfoResponse { Language = user.Language };
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
    }
}
