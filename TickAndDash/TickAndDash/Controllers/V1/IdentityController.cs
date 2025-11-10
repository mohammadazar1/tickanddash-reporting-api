using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Extensions;
using TickAndDash.Filters;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using static TickAndDash.Filters.AuthorizationFilter;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class IdentityController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly ISMSService _sMSService;
        private readonly IBulkSMSService _bulkSMSService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICarService _carService;
        private readonly IBlacklistedService _blacklistedService;
        private readonly ICarsQueueService _carsQueueService;
        //private readonly IRidersQueueService _ridersQueueService;
        private readonly IStringLocalizer<IdentityController> _localizer;
        private readonly IWalletService _walletService;
        private ILogger<IdentityController> _logger;
        private readonly ISystemConfigurationDAL _ISystemConfigurationService;
        public IdentityController(IUsersService usersService, IMapper mapper, ISMSService sMSService, IActionContextAccessor actionContextAccessor,
            ICarService carService, IBlacklistedService blacklistedService, ICarsQueueService carsQueueService, IStringLocalizer<IdentityController> localizer,
            //IRidersQueueService ridersQueueService,
            IWalletService walletService,
            IBulkSMSService bulkSMSService,
            ILogger<IdentityController> logger,
            ISystemConfigurationDAL SystemConfigurationService
            )
        {
            _usersService = usersService;
            _mapper = mapper;
            _sMSService = sMSService;
            _actionContextAccessor = actionContextAccessor;
            _carService = carService;
            _blacklistedService = blacklistedService;
            _carsQueueService = carsQueueService;
            _localizer = localizer;
            //_ridersQueueService = ridersQueueService;
            _walletService = walletService;
            _bulkSMSService = bulkSMSService;
            _logger = logger;
            _ISystemConfigurationService = SystemConfigurationService;
        }


        #region TO DO
        // Limits for Numbers of pincodes from the same number, let us check 
        // Check if it is mobile device or not
        // Validate token in Auth Service
        #endregion
        /// <summary>
        /// Request for sending the Pincode to riders to validate their logging to the application, used for registeration too (Wallet,tickAndDash Registeration) 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.MobVal_1 Mobile Number is not valid<br/>
        /// 2.Ok Success<br/>
        /// 3.blok_1 user is blocked from using the service<br/>
        /// 4.MobVal_2 request is not comming from Mobile device<br/>
        /// 5.param_1 BadRequest by missing paramter request or passing unvalid value<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response>
        /// <response code = "404"> Failed </response>
        /// <response code = "403"> Failed </response>
        [HttpPost("Riders/Login/Pincode")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> Sendpincode([FromBody] RidersSendPincodeRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null,
                Code = ""
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            bool isValidMobileNumber = request.MobileNumber.IsPalMobileNumber();
            if (!isValidMobileNumber)
            {
                response.Message = _localizer["InvalidMSISDNMsg"].Value; /* "عذرًا، يرجى التاكد من رقم الموبايل المدخل";*/
                response.Code = ValidationCodes.MobVal_1.ToString();
                return BadRequest(response);
            }

            request.MobileNumber = request.MobileNumber.ConvertMobileNumberFormate();

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

            //Log.
            //    ForContext("MSISDN", request.MobileNumber).
            //    Information("generate a pin code rquest started");
            var rider = await _usersService.GetRiderByMobileNumberAsync(request.MobileNumber);
            if (rider?.User == null)
            {
                rider = _mapper.Map<Riders>(request);
                string password = Helpers.CreatePassword(8);
                //string username = Helpers.RandomString(14);
                var registerUserResponse = await _walletService.
                    RegisterUserAsync($"{request.MobileNumber}@PalCap",
                    request.MobileNumber, password);

                if (registerUserResponse == null || registerUserResponse.Success == false
                    || string.IsNullOrWhiteSpace(registerUserResponse.Data?.Token))
                {
                    response.Message =
                        !string.IsNullOrWhiteSpace(registerUserResponse.Data?.Token) ?
                        registerUserResponse.MessageAr :
                        "عذرًا، حدث خطأ ما يرجى مراجعة ادارة التطبيق!";

                    response.Code = WalletError.Wallet1.ToString();
                    //Helpers.LogReponse(response, "GeneratePinResponse", "Generate Pin Code Response");
                    return UnprocessableEntity(registerUserResponse);
                }

                #region production SMS sender
                //if (registerUserResponse.Code == "Valdata2")
                //{
                //    Task.Run(async () =>
                //  {
                //      //_sMSService.SendSMSToUserAsync(rider.MobileNumber, $"لقد تم تفعيل المحفظة الالكترونية الخاصة بك");
                //      await _bulkSMSService.SendSMSAsync(
                //          new Services.ServicesDtos.SMSDto
                //          {
                //              MSISDN = rider.MobileNumber,
                //              Msg = $"لقد تم تفعيل المحفظة الالكترونية الخاصة بك"
                //          });
                //  });
                //}
                //else
                //{
                //    Task.Run(async () =>
                // {
                //     //_sMSService.SendSMSToUserAsync(rider.MobileNumber, $"تم تسجيل الحافظة الخاصة بك {Environment.NewLine}" +
                //     //       $"اسم المستخدم: {username} {Environment.NewLine}" +
                //     //       $"رمز المرور: {password} {Environment.NewLine}" +
                //     //       $"عليك عدم مشاركة هذه المعلومة مع احد");

                //     await _bulkSMSService.SendSMSAsync(
                //        new Services.ServicesDtos.SMSDto
                //        {
                //            MSISDN = rider.MobileNumber,
                //            Msg = $"تم تسجيل الحافظة الخاصة بك {Environment.NewLine}" +
                //           $"اسم المستخدم: {username} {Environment.NewLine}" +
                //           $"رمز المرور: {password} {Environment.NewLine}" +
                //           $"عليك عدم مشاركة هذه المعلومة مع أحد"
                //        });
                // });
                //}
                #endregion
                rider.Token = registerUserResponse.Data.Token;
                rider.User.Id = await _usersService.AddRiderAsync(rider);
            }
            else
            {
                bool isBlocked = await _blacklistedService.IsUserBlockedAsync(rider.UserId);

                if (isBlocked)
                {
                    response.Message = _localizer["BlackListedMSISDNMsg"].Value;
                    response.Code = ValidationCodes.blok_1.ToString();
                    //Helpers.LogReponse(response, "GeneratePinResponse", "Generate Pin Code Response");
                    return StatusCode(403, response);
                }

                do
                {
                    rider.GeneratedPincode = Helpers.GenerateRandomPinCode();
                } while (rider.LoginPincode == rider.GeneratedPincode);

                // No Need
                await _usersService.UpdateRiderGeneratedPincodeAsync(rider);
            }

            // Task.Run(async () =>
            //{
            
            await _sMSService.SendSMSToUserAsync(rider.MobileNumber, $"Code {rider.GeneratedPincode.ToString()}");

            // await _bulkSMSService.SendSMSAsync(new Services.ServicesDtos.SMSDto
            //{
            //    MSISDN = rider.MobileNumber,
            //    Msg = $"Code {rider.GeneratedPincode.ToString()}"
            //});
            //});
            
            response.Message = _localizer["SuccessPincodeMsg"].Value; /*"تم إرسال رمز التفعيل بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            //Helpers.LogReponse(response, "GeneratePinResponse", "Generate Pin Code Response");
            
            return Ok(response);
        }


        /// <summary>
        /// Riders request for logging to the application by submitting the pin code
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Ok Success<br/>
        /// 2.MobVal_1 Mobile Number is not valid<br/>
        /// 3.Usr_2  User not found
        /// 3.Pin_2 pincode Expired<br/>
        /// 4.Pin_1 wrong pin code<br/>
        /// 5.seq_1  database failure<br/>
        /// 6.param_1 BadRequest by missing paramter request or passing unvalid value<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response>
        /// <response code = "422"> Failed </response>
        /// <response code = "404"> Failed </response>
        /// <response code = "409"> Failed </response>
        /// <response code = "500"> Internal Error </response>
        [HttpPut("Riders/Login/Pincode/Confirmation")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<LoginResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> Login([FromBody] RidersLoginRequest request)
        {
            GeneralResponse<LoginResponse> response = new GeneralResponse<LoginResponse>()
            {
                Data = null
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }


            bool isValidMobileNumber = request.MobileNumber.IsPalMobileNumber();
            if (!isValidMobileNumber)
            {
                response.Message = _localizer["InvalidMSISDNMsg"].Value;
                response.Code = ValidationCodes.MobVal_1.ToString();
                return BadRequest(response);
            }
            request.MobileNumber = request.MobileNumber.ConvertMobileNumberFormate();

            //Log.
            //ForContext("MSISDN", request.MobileNumber).
            //Information("confirm pin code rquest started");

            if (!Helpers.IsValidMobileOS(request.MobileOperatingSystem))
            {
                response.Message = "MobileOperatingSystem is equal to (iOS/android)";
                response.Code = ValidationCodes.Param_1.ToString();
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return BadRequest(response);
            }

            var rider = await _usersService.
                              GetRiderByMobileNumberAsync(request.MobileNumber);

            if (rider?.User == null)
            {
                response.Message = _localizer["UnregisteredAccountMsg"].Value;
                //*"هذا الحساب غير معرف"*/;
                response.Code = UserCodes.Usr_2.ToString();
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return Conflict(response);
            }

            // trying to use the same pincode twice 
            if (rider.GeneratedPincode == rider.LoginPincode)
            {
                response.Code = ValidationCodes.Pin_2.ToString();
                response.Message = _localizer["ExpiredPincodeMsg"].Value; /*"عذرً، انتهت فعالية رمز التفعيل يرجى اعادة المحاولة"*/;
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return StatusCode(422, response);
            }

            // wrong pinCode
            if (rider.GeneratedPincode != request.Pincode)
            {
                response.Code = ValidationCodes.Pin_1.ToString();
                response.Message = _localizer["WrongPincodeMsg"].Value; /* "عذرًا، رمز التفعيل الذي أرسلته خاطئ"*/;
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return BadRequest(response);
            }

            if (rider.User.Token != null)
            {
                bool isTokenExpired = await _blacklistedService.
                    ExpireTokenAsync(rider.UserId, rider.User.Token);
                if (!isTokenExpired)
                {
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    response.Message = _localizer["GeneralErrorMsg"].Value;
                    //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                    return UnprocessableEntity(response);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.FCMToken) && rider.User.FCMToken != request.FCMToken)
            {
                bool isFCMTokenUpdate = await _usersService.UpdateUserFCMTokenAsync(rider.UserId, request.FCMToken);

                if (!isFCMTokenUpdate)
                {
                    response.Message = _localizer["GeneralErrorMsg"].Value;
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                    return UnprocessableEntity(response);
                }
            }

            string token = _usersService.CreateUserToken(rider, request.FCMToken);

            bool isTokenUpdated = await _usersService.UpdateUserTokenAsync(rider.UserId, token);
            if (!isTokenUpdated)
            {
                response.Code = Generalcodes.Internal.ToString();
                response.Message = _localizer["GeneralErrorMsg"].Value;
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return UnprocessableEntity(response);
            }

            try
            {
                var deviceInfo = _actionContextAccessor.GetDeviceInfo();
                await _usersService.AddUserSessionAsync(rider.UserId, deviceInfo);

            }
            catch (Exception ex)
            {
                Log.
                    ForContext("Exception", ex).
                    Error("Exception when confirming the pincode");
                //_logger.LogError($"{ex.ToString()}");
            }

            if (!rider.IsSubscribed &&
                (rider.UnsubDate == null || rider.UnsubDate == default(DateTime)))
            {
                var isSubscribed = await _walletService.SubscribeAysnc(new SubscribeRequest
                {
                    ServiceId = 1,
                    Token = rider.Token
                });

                if (isSubscribed != null && !isSubscribed.Success)
                {
                    Log.
                    ForContext("MSISDN", request.MobileNumber).
                    Error("Exception when subscriping the user");
                    //_logger.LogWarning(@$"{request.MobileNumber} failed to subscribe, subscription response {isSubscribed?.MessageAr} code: ${isSubscribed?.Code}");
                }
            }

            // so we inti only one session
            rider.LoginPincode = request.Pincode;
            rider.MobileOS = request.MobileOperatingSystem;

            bool isRiderInfoUpdated = await _usersService.UpdateRiderAsync(rider);
            if (!isRiderInfoUpdated)
            {
                response.Code = UnExpectedErrors.sql_1.ToString();
                response.Message = _localizer["GeneralErrorMsg"].Value;
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return BadRequest(response);
            }

            //await _ridersQueueService.CancelRiderReservationIfAnyAsync(rider.User.Id);

            response.Success = true;
            response.Message = _localizer["SuccessLoginMsg"].Value; /*"تم تسجيل الدخول بنجاح"*/;
            response.Code = Generalcodes.Ok.ToString();
            response.Data = new LoginResponse
            {
                Token = token
            };

            //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
            return Ok(response);
        }


        /// <summary>
        /// Drivers Request for Logging to the application by submitting their License Number and passwords
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **codes:**<br/>
        /// 1.Ok Success<br/>
        /// 2.Usr_2  user Not found<br/>
        /// 3.param_1 BadRequest by missing paramter request or passing unvalid value<br/>
        /// 4.seq_1  database failure<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response>
        /// <response code = "404"> Failed </response>
        /// <response code = "422"> Failed </response>
        [HttpPost("Drivers/Login")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<LoginResponse>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> DriversLogin([FromBody] DriversLoginRequest request)
        {
            GeneralResponse<LoginResponse> response = new GeneralResponse<LoginResponse>()
            {
                Data = null,
                Code = ""
            };
            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            if (!string.IsNullOrWhiteSpace(request.MobileOS)
                && !Helpers.IsValidMobileOS(request.MobileOS))
            {
                response.Message = "MobileOperatingSystem is equal to (iOS/android)";
                response.Code = ValidationCodes.Param_1.ToString();
                //Helpers.LogReponse(response, "confirmPinResponse", "confirm Pin Response");
                return BadRequest(response);
            }

            var driver = await _usersService.
                GetDriverByLicenseNumberAsync(request.LicenseNumber);

            if (driver == null)
            {
                response.Message = _localizer["InvalidCredentialsMsg"].Value; /* "عذرًا، يرجى التاكد من اسم المستخدم و رمز المرور"*/;
                response.Code = UserCodes.Usr_2.ToString();
                Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                return StatusCode(404, response);
            }

            bool isCorrectPassword = await _usersService.IsDriverPasswordCorrectAsync(driver.UserId, request.Password);
            if (!isCorrectPassword)
            {
                response.Message = _localizer["InvalidCredentialsMsg"].Value;// "عذرًا، يرجى التاكد من اسم المستخدم و رمز المرور";
                response.Code = UserCodes.Usr_2.ToString();
                //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                return StatusCode(404, response);
            }

            var isActive = await _usersService.IsDriverActiveAsync(driver.UserId);
            if (!isActive)
            {
                response.Message = _localizer["IsActiveDriver"].Value;
                response.Code = ValidationCodes.Active_1.ToString();
                //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                return StatusCode(422, response);
            }

            if (driver.User.Token != null)
            {
                bool isTokenExpired = await _blacklistedService.ExpireTokenAsync(driver.UserId, driver.User.Token);
                if (!isTokenExpired)
                {
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    response.Message = _localizer["GeneralErrorMsg"].Value; //* "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا!";*/
                    //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                    return UnprocessableEntity(response);
                }
            }

            string token = _usersService.CreateUserToken(driver, request.FCMToken);
            bool isTokenUpdated = await _usersService.UpdateUserTokenAsync(driver.UserId, token);

            if (!isTokenUpdated)
            {
                response.Code = UnExpectedErrors.sql_1.ToString();
                response.Message = _localizer["GeneralErrorMsg"].Value; // "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا!";
                Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                return UnprocessableEntity(response);
            }

            int CarDrivesCount = await _carService.GetCarDriversCountAsync(driver.CarId);

            if (CarDrivesCount == 1)
            {
                bool isUpdated = await _carService.
                    UpdateLoggedInCarUserAsync(driver.CarId, driver.UserId);

                if (!isUpdated)
                {
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    response.Message = _localizer["GeneralErrorMsg"].Value; //"عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا!";
                    //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                    return UnprocessableEntity(response);
                }
            }

            try
            {
                var deviceInfo = _actionContextAccessor.GetDeviceInfo();
                await _usersService.AddUserSessionAsync(driver.UserId, deviceInfo);
                // log warning
            }
            catch (Exception ex)
            {
                Log.
                    ForContext("GetDeviceInfo", ex, true).
                    Error("GetDeviceInfo() Error");
                //Log.ForContext()
                //_logger.LogError($"{ex.ToString()}");
                // log warrning
            }

            if (string.IsNullOrWhiteSpace(request.FCMToken))
            {
                //Log
                // Log Warning 
            }

            if (!string.IsNullOrWhiteSpace(request.FCMToken) && driver.User.FCMToken != request.FCMToken)
            {
                bool isFCMTokenUpdate = await _usersService.UpdateUserFCMTokenAsync(driver.UserId, request.FCMToken);
                if (!isFCMTokenUpdate)
                {
                    response.Message = _localizer["GeneralErrorMsg"].Value; // "عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا!";
                    response.Code = UnExpectedErrors.sql_1.ToString();
                    //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");
                    return UnprocessableEntity(response);
                }
            }
            if (!string.IsNullOrWhiteSpace(request.MobileOS))
            {
                await _usersService.UpdateDriverMobileOs(driver.UserId, request.MobileOS);
            }

            response.Data = new LoginResponse();
            response.Success = true;
            response.Message = _localizer["SuccessLoginMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Data.Token = token;
            //Helpers.LogReponse(response, "DriverLoginRespionse", "Driver Login Response");

            return Ok(response);
        }

        #region To DO
        // check user session table if he has an open sessesion
        #endregion
        /// <summary>
        ///  Request for logging out from the application for drivers and riders
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// On logout driver will be logged out from the queue
        /// **codes:**<br/>
        /// 1.Ok  Success<br/>
        /// 2.LogOut_1  user is not logged in<br/>
        /// 3.Auth_1  token is not valid<br/>
        /// 4.Auth_2  rider session is expired due to opening another one<br/>
        /// 5.seq_1  database failure<br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "422"> Failed </response>
        /// <response code = "403"> Failed </response>
        [HttpPut("Logout")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider, Driver")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 1/*, Arguments = new object[] { RolesEnum.Rider }*/)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> LogOut()
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null,
                Code = ""
            };

            string role = User.FindFirstValue(ClaimTypes.Role);
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            RolesEnum roleEnum = (RolesEnum)Enum.Parse(typeof(RolesEnum), role, true);

            if (roleEnum == RolesEnum.Driver)
            {
                int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
                var carQ = await _carsQueueService.GetActiveCarInCarQAsync(carId);

                if (carQ != null && carQ.Turn == 1 && userId == carQ.Car.LoggedInDriverId)
                {
                    response.Message = _localizer["YourTurnValidation"].Value;
                    response.Code = QueusCode.car_4.ToString();
                    return UnprocessableEntity(response);
                }

                if (carQ != null && carQ.Turn != 1)
                {
                    await _carService.SwtichLoggedInCarUserAsync(carId, userId);
                    bool isCanceled = await _carsQueueService.CancelDriverCarReservationIfAnyAsync(carId);
                    if (!isCanceled)
                    {
                        response.Message = _localizer["GeneralErrorMsg"].Value;
                        response.Code = UnExpectedErrors.sql_1.ToString();
                        return StatusCode(422, response);
                    }
                }
            }

            //else if (roleEnum == RolesEnum.Rider)
            //{
            //    await _ridersQueueService.CancelRiderReservationIfAnyAsync(userId);
            //}

            bool isBlocked = await _blacklistedService.ExpireTokenAsync();
            if (!isBlocked)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return StatusCode(422, response);
            }

            await _usersService.UpdateUserSessionLogoutDatetimeAsync(userId);

            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            response.Message = _localizer["SuccessLogoutMsg"].Value;/* "تم تسجيل الخروج بنجاح";*/
            return Ok(response);
        }

        //// No needs for it now 
        //private void SetCookies(string key, string value)
        //{
        //    CookieOptions option = new CookieOptions();
        //    option.HttpOnly = true;
        //    option.SameSite = SameSiteMode.Lax;
        //    Response.Cookies.Append(key, value, option);
        //}
        //private bool IsLogedIn()
        //{
        //    return Request.Cookies.ContainsKey("token");
        //}
        //private void RemoveCookies(string key)
        //{
        //    Response.Cookies.Delete("token");
        //}
    }
}
