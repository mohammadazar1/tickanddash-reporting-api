using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
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
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using static TickAndDash.Filters.AuthorizationFilter;
using TransferBalanceRequest = TickAndDash.HttpClients.GeoClients.DTOs.TransferBalanceRequest;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class WalletController : ControllerBase
    {

        private readonly IUsersService _usersService;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<WalletController> _localizer;
        private readonly IUserTransactionsService _userTransactionsService;
        private readonly ICarService _carService;
        //private readonly IFCMHttpClient _fCMHttpClient;
        private readonly IRefillRequestsService _refillRequestsService;
        private readonly INotificationService _notificationService;

        public WalletController(IUsersService usersService, IWalletService walletService, IMapper mapper,
            IStringLocalizer<WalletController> localizer, IUserTransactionsService userTransactionsService, ICarService carService, IRefillRequestsService refillRequestsService, INotificationService notificationService)
        {
            _usersService = usersService;
            _walletService = walletService;
            _mapper = mapper;
            _localizer = localizer;
            _userTransactionsService = userTransactionsService;
            _carService = carService;
            _refillRequestsService = refillRequestsService;
            _notificationService = notificationService;
        }

        [HttpGet("Balance")]
        [Authorize(Roles = "Driver,Rider")]
        [MapToApiVersion("1.0")]
        //[ProducesResponseType(typeof(GeneralResponse<object>), 200)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetUserBalance([FromQuery] GetUserBalanceRequest request)
        {
            GeneralResponse<GetUserBalanceResponse> response = new GeneralResponse<GetUserBalanceResponse>()
            {
                Message = _localizer["GeneralMsg"],
                Data = new GetUserBalanceResponse() { CurrencyCode = "ILS" },
                Code = ""
            };
            //bool isToDateFormateValid = DateTime.TryParse(request.FromDate, new CultureInfo("en"), System.Globalization.DateTimeStyles.None,
            // out DateTime fromDate);
            bool isToDateFormateValid = DateTime.TryParse(request.FromDate, out DateTime fromDate);
            //bool isDateToFormateValid = DateTime.TryParse(request.ToDate, new CultureInfo("en"), System.Globalization.DateTimeStyles.None,
            bool isDateToFormateValid = DateTime.TryParse(request.ToDate, out DateTime toDate);
            if (!isToDateFormateValid || !isToDateFormateValid)
            {
                response.Message = "";
                return UnprocessableEntity(response);
            }

            string msisdn = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            string userId = User.FindFirstValue(ClaimsEnum.UserId.ToString());
            string role = User.FindFirstValue(ClaimTypes.Role);

            RolesEnum roleEnum = (RolesEnum)Enum.Parse(typeof(RolesEnum), role, true);
            string token = "";

            if (roleEnum is RolesEnum.Rider)
            {
                var rider = await _usersService.GetRiderByMobileNumberAsync(msisdn);
                token = rider.Token;
            }

            if (roleEnum is RolesEnum.Driver)
            {
                string licenseNumber = User.FindFirstValue(ClaimsEnum.LicenseNumber.ToString());
                var driver = await _usersService.GetDriverByLicenseNumberAsync(licenseNumber);
                token = driver.Token;
            }

            var balance = await _walletService.GetUserBalanceAsync(token);
            if (balance != null && balance.Code == "Valdata1")
            {
                response.Success = true;
                response.Message = _localizer["SuccessMsg"].Value;
                return Ok(response);
            }
            else if (balance == null || balance.Success == false)
            {
                response.Success = true;
                response.Message = Request.Headers?["Content-Language"].ToString() == "ar" ? balance.MessageAr : balance.MessageEn;
                response.Code = "";
                return UnprocessableEntity(response);
            }


            var ILSWallet = balance.Data.Find(x => x.CurrencyCode == "ILS");
            if (ILSWallet != null)
            {
                response.Data = _mapper.Map<GetUserBalanceResponse>(ILSWallet);
            }

            var userTransactions = await _userTransactionsService.GetUserTransactionsAsync(int.Parse(userId), fromDate, toDate);
            if (userTransactions != null && userTransactions.Any())
            {
                response.Data.UserTransactions = _mapper.Map<List<UserTransactionsData>>(userTransactions);
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Success = true;

            return Ok(response);
        }


        [HttpPost("Transfer")]
        [Authorize(Roles = "Rider,Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> TransferBalance([FromBody] TransferBalanceRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>();
            response.Message = _localizer["GeneralMsg"].Value;
            response.Code = Generalcodes.Failed.ToString();

            string msisdn = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            string role = User.FindFirstValue(ClaimTypes.Role).ToString();
            RolesEnum roleEnum = (RolesEnum)Enum.Parse(typeof(RolesEnum), role, true);

            bool isValidMobileNumber = request.MobileNumber.IsPalMobileNumber();
            if (!isValidMobileNumber)
            {
                response.Message = _localizer["InvalidMSISDNMsg"].Value; /* "عذرًا، يرجى التاكد من رقم الموبايل المدخل";*/
                response.Code = ValidationCodes.MobVal_1.ToString();
                return BadRequest(response);
            }
            string fcm = "";

            request.MobileNumber = request.MobileNumber.ConvertMobileNumberFormate();
            var receiverUser = await _usersService.GetUserIdByMobileNumberAsync(request.MobileNumber, RolesEnum.Unknown);
            int receiverUserId = receiverUser.Item1;
            fcm = receiverUser.Item2;
            string language = receiverUser.Item3;

            var senderUser = await _usersService.GetUserIdByMobileNumberAsync(msisdn, roleEnum);
            int senderUserId = senderUser.Item1;

            //var receiver = _usersService.GetRiderByMobileNumberAsync(request.MobileNumber).Result;
            //var sender = _usersService.GetRiderByMobileNumberAsync(msisdn).Result;
            //if (receiver != null)

            //string language = "ar";
            string mobileType = "";
            if (receiverUserId > 0)
            {
                if (roleEnum == RolesEnum.Driver)
                {
                    var driver = await _usersService.GetDriverByUserIdAsync(senderUserId);
                    request.Token = driver.Token;
                    mobileType = driver.MobileOS;
                }
                else
                {
                    var rider = await _usersService.GetRiderByIdAsync(senderUserId);
                    request.Token = rider.Token;
                }

                //request.Token = sender.Token;

                var transferResult = await _walletService.TransferBalanceAsync(request);
                // insert the record in the DB
                if (transferResult.Success)
                {
                    await _userTransactionsService.AddUserTransactionAsync(new UserTransactions
                    {
                        CreationDate = DateTime.Now,
                        FromUserId = senderUserId,
                        //FromUserId = sender.UserId,
                        ToUserId = receiverUserId,
                        //ToUserId = receiver.UserId,
                        Type = UserTransactionsTypesEnum.Transfer.ToString(),
                        Amount = request.TransferBalance,
                        UserTransactionTypeId = (int)UserTransactionsTypesEnum.Transfer
                    });

                    string lang = language;

                    string title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgTitle_ar :
                           NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgTitle_en;

                    string body = lang == "ar" ?
                           string.Format(NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgBody_ar, request.TransferBalance) :
                           string.Format(NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgBody_en,
                            request.TransferBalance);

                    await _notificationService.SendNotificationAsync(fcm
                                   , body, title, "", "Transfer", mobileType, RolesEnum.Rider);

                    //response.Success = true; ;
                    //response.Message = "تم معالجة طلبك بنجاح";
                    //response.Code = Generalcodes.Ok.ToString();
                }
                else
                {
                    response.Message = Request.Headers?["Content-Language"].ToString() == "ar" ? transferResult.MessageAr : transferResult.MessageEn;
                    response.Code = "";

                    string lang = language;

                    string title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgTitle_ar :
                           NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgTitle_en;
                    string body = response.Message;
                    await _notificationService.SendNotificationAsync(fcm
                                   , body, title, "", "Transfer", mobileType, RolesEnum.Rider);
                    return UnprocessableEntity(response);
                }
            }
            else
            {
                response.Code = ValidationCodes.MobVal_1.ToString();
                response.Message = "رقم الهاتف المرسل غير فعال";

                return BadRequest(response);
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }


        [HttpPost("UnSubscribe")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> UnSubscribe()
        {
            GeneralResponse<object> response = new GeneralResponse<object>();
            response.Message = _localizer["GeneralMsg"].Value;
            response.Code = "";

            string msisdn = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            //string userId = User.FindFirstValue(ClaimsEnum.UserId.ToString());
            var rider = await _usersService.GetRiderByMobileNumberAsync(msisdn);

            if (!rider.IsSubscribed)
            {
                response.Message = _localizer["NotSubscribedMsg"].Value;
            }

            var isSubscribed = await _walletService.UnSubscribeAysnc(new SubscribeRequest
            {
                Token = rider.Token,
                ServiceId = 1
            });


            if (isSubscribed == null || isSubscribed.Success == false)
            {
                response.Message = Request.Headers?["Content-Language"].ToString() == "ar" ? isSubscribed.MessageAr : isSubscribed.MessageEn;
                response.Code = "";
                return UnprocessableEntity(response);
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }

        [HttpGet("Subscription/Status")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> IsSubscribe()
        {
            GeneralResponse<IsSubscribeResponse> response = new GeneralResponse<IsSubscribeResponse>
            {
                Data = new IsSubscribeResponse
                {
                    Status = SubStatus.Unsub.ToString()
                }
            };

            response.Message = _localizer["GeneralMsg"].Value;
            response.Code = "";
            response.Success = true;

            string msisdn = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            var rider = await _usersService.GetRiderByMobileNumberAsync(msisdn);

            response.Data.ActiveSubscriptionPeriod = rider.ActiveSubscriptionPeriod;
            if (rider.ActiveSubscriptionPeriod >= DateTime.Now)
            {
                if (rider.IsSubscribed)
                {
                    response.Data.Status = SubStatus.Sub.ToString();
                    response.Message = _localizer["ActiveSub", rider.ActiveSubscriptionPeriod.ToString("yyyy-MM-dd")].Value;
                    response.Code = Generalcodes.Ok.ToString();
                }
                else
                {
                    response.Data.Status = SubStatus.UnsubActive.ToString();
                    response.Message = _localizer["UnSubActive"].Value;
                    response.Code = Generalcodes.Sub_3.ToString();
                }
            }
            else
            {
                if (rider.IsSubscribed)
                {
                    response.Data.Status = SubStatus.SubNoBalance.ToString();
                    response.Message = _localizer["NotActiveSub"].Value;
                    response.Code = Generalcodes.Sub_2.ToString();
                }
                else
                {
                    response.Data.Status = SubStatus.Unsub.ToString();
                    response.Message = _localizer["NotSubscribedMsg"].Value;
                    response.Code = Generalcodes.Sub_3.ToString();
                }
            }

            return Ok(response);
        }

        [HttpPost("Subscribe")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> Subscribe()
        {
            GeneralResponse<SubscribeResponse> response = new GeneralResponse<SubscribeResponse>
            {
                Success = true,
                Data = new SubscribeResponse
                {
                    Subscribed = false,
                }
            };

            response.Message = _localizer["GeneralMsg"].Value;
            response.Code = "";

            string msisdn = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            var rider = await _usersService.GetRiderByMobileNumberAsync(msisdn);

            var subResponse = await _walletService.SubscribeAysnc(new SubscribeRequest
            {
                ServiceId = 1,
                Token = rider.Token,
                Language = Request.Headers?["Content-Language"]
            });

            response.Message = Request.Headers?["Content-Language"].ToString() == "ar" ? subResponse.MessageAr : subResponse.MessageEn;
            response.Code = subResponse.StatusCode ?? "";

            if (subResponse.StatusCode == "NoBalance")
            {
                response.Code = SubStatus.SubNoBalance.ToString();
                response.Data.Subscribed = true;
                response.Data.Status = SubStatus.SubNoBalance.ToString();
            }

            else if (subResponse.StatusCode == "SubSuccess")
            {
                response.Code = SubStatus.Sub.ToString();
                response.Data.Subscribed = true;
                response.Data.Status = SubStatus.Sub.ToString();
            }
            else if (subResponse.StatusCode == "SubRestored")
            {
                response.Code = SubStatus.SubRestored.ToString();
                response.Data.Subscribed = true;
                response.Data.Status = SubStatus.SubRestored.ToString();

            }
            else if (subResponse.StatusCode == "AlreadySubscribed")
            {
                response.Code = SubStatus.Sub.ToString();
                response.Data.Subscribed = true;
                response.Data.Status = SubStatus.Sub.ToString();
            }
            else if (subResponse.Success)
            {
                response.Success = true;
                response.Data.Subscribed = true;
            }

            return Ok(response);
        }

        /// <summary>
        /// Get Count of Driver Refilling Requests in the past hour to be displayed in notification bar
        /// </summary>
        /// <returns></returns>
        [HttpGet("Refilling/Notifications/Count")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetCountOfRefillingNotifications()
        {

            GeneralResponse<RefillingCountResponse> response = new GeneralResponse<RefillingCountResponse>
            {
                Success = true,
                Code = "",
                Data = new RefillingCountResponse { RefillingCount = 0 }
            };

            int driverId = int.Parse(User.FindFirstValue(ClaimsEnum.UserId.ToString()));
            //var driver = await _usersService.GetDriverByUserIdAsync(driverId);

            int refillingCount = await _refillRequestsService.GetCountOfDriverRefillRequestsAsync(driverId);
            if (refillingCount > 0)
            {
                response.Data.RefillingCount = refillingCount;
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }

        /// <summary>
        /// Get All Driver Refilling Requests in the past hour 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Refilling/Requests/All")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> GetAllRefillingRequests()
        {
            GeneralResponse<List<GetAllRefillingRequestsResponse>> response = new GeneralResponse<List<GetAllRefillingRequestsResponse>>
            {
                Success = true,
                Code = "",
                Data = new List<GetAllRefillingRequestsResponse> { }
            };

            int driverId = int.Parse(User.FindFirstValue(ClaimsEnum.UserId.ToString()));

            var refillingRequests = await _refillRequestsService.GetAllRefillRequestsAsync(driverId);

            if (refillingRequests != null && refillingRequests.Any())
            {
                response.Data = _mapper.Map<List<GetAllRefillingRequestsResponse>>(refillingRequests);
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }

        [HttpPost("Requests/Refilling")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> SendRequestToDriverForRefillingTheWallet(RequestRefill request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>
            {
                Success = false,
                Code = ""
            };

            int riderId = int.Parse(User.FindFirstValue(ClaimsEnum.UserId.ToString()));

            int carId = await _carService.GetCarIdByCarCodeAsync(request.CarNumber);

            if (carId < 1)
            {
                response.Message = _localizer["WrongCarCode"].Value;
                return BadRequest(response);
            }

            bool isCarActive = await _carService.IsCarActiveByCarCodeAsync(request.CarNumber);
            if (!isCarActive)
            {
                response.Message = _localizer["WrongCarCode"].Value;
                return BadRequest(response);
            }

            int activeDriverId = await _carService.GetCarActiveDriverIdByCarIdAsync(carId);
            if (activeDriverId < 1)
            {
                response.Message = _localizer["NotActiveDriver"].Value;
                return BadRequest(response); ;
            }

            var driver = await _usersService.GetDriverByUserIdAsync(activeDriverId);

            bool isRequestValid = await _refillRequestsService.IsThereAnyRequestsToTransferInTheLastNminutesAsync(riderId, driver.UserId);
            if (isRequestValid)
            {
                response.Message = _localizer["LimitToRefillRequest", 5].Value;
                return BadRequest(response);
            }
            else
            {
                var balance = await _walletService.GetUserBalanceAsync(driver.Token);

                if (balance == null || balance.Success == false)
                {
                    response.Message = _localizer["BalnceNotEnough"].Value;
                    response.Code = "";
                    return UnprocessableEntity(response);
                }

                var ILSWallet = balance.Data.Find(x => x.CurrencyCode == "ILS");
                if (ILSWallet != null)
                {
                    var wallet = _mapper.Map<GetUserBalanceResponse>(ILSWallet);

                    if (wallet?.Balance < request.Amount)
                    {
                        response.Message = _localizer["BalnceNotEnough"].Value;
                        response.Code = "";
                        return BadRequest(response);
                    }
                }

                RefillRequest refillRequest = new RefillRequest
                {
                    RiderId = riderId,
                    DriverId = driver.UserId,
                    Amount = request.Amount,
                    CreatedAt = DateTime.Now,
                };

                await _refillRequestsService.InsertRefillRequest(refillRequest);
            }


            var rider = await _usersService.GetRiderByIdAsync(riderId);
            
            string lang = driver.User.Language;
            string title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationMsgTitle_ar :
                    NotificationTexts.WalletNotification.RefillNotificationMsgTitle_en;
            string body = lang == "ar" ?
                string.Format(NotificationTexts.WalletNotification.RefillNotificationMsgBody_ar, rider.MobileNumber, request.Amount) :
                    string.Format(NotificationTexts.WalletNotification.RefillNotificationMsgBody_en, rider.MobileNumber, request.Amount);


            bool isSent = await _notificationService.SendNotificationAsync(driver.User.FCMToken
                  , body, title, "", "Refill",
                  driver.MobileOS, RolesEnum.Driver, riderId, request.Amount);

            //PushNotificationDto pushNotificationDto = new PushNotificationDto()
            //{
            //    data = new Data
            //    {
            //        title = title,
            //        body = body,
            //        category = "Refill",
            //        click_action = "VIEW_NOTIFICATION",
            //        RiderId = riderId.ToString(),
            //        Amount = request.Amount.ToString()
            //    },
            //    notification = new Notification
            //    {
            //        title = title,
            //        body = body,
            //        category = "Refill",
            //        click_action = "VIEW_NOTIFICATION",
            //    },
            //    to = driver.User.FCMToken
            //};

            // hangfire
            //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
            //bool isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

            if (!isSent)
            {
                // send warning
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }


        [HttpPost("Requests/Refilling/Response")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> DriverAcceptingRiderRefillingRequest([FromBody] RefillResponse request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>
            {
                Success = false,
                Code = ""
            };

            int driverId = int.Parse(User.FindFirstValue(ClaimsEnum.UserId.ToString()));
            var refillRequest = await _refillRequestsService.GetRefillRequest(request.RiderId, driverId, request.Amount);

            if (refillRequest == null)
            {
                response.Message = _localizer["WrongDataMsg"].Value;
                return BadRequest(response);
            }

            string lang;
            string title;
            string body;
            bool isSent;
            //PushNotificationDto pushNotificationDto;
            var driver = await _usersService.GetDriverByUserIdAsync(driverId);
            var rider = await _usersService.GetRiderByIdAsync(request.RiderId);

            if (!request.IsSuccess)
            {

                await _refillRequestsService.UpdateRefillRequest(new RefillRequest
                {
                    RiderId = request.RiderId,
                    DriverId = driverId,
                    Amount = request.Amount,
                    IsHandled = true,
                    IsSuccess = false
                });


                lang = rider.User.Language;
                title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationMsgTitle_ar :
                       NotificationTexts.WalletNotification.RefillNotificationMsgTitle_en;
                body = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationFailedResponseMsgBody_ar :
                   NotificationTexts.WalletNotification.RefillNotificationFailedResponseMsgBody_en;


                isSent = await _notificationService.SendNotificationAsync(rider.User.FCMToken
          , body, title, "", "RefillResponse",
          "", RolesEnum.Rider);

                //pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = title,
                //        body = body,
                //        category = "RefillResponse",
                //    },
                //    notification = new Notification
                //    {
                //        title = title,
                //        body = body,
                //        category = "RefillResponse",
                //    },
                //    to = rider.User.FCMToken
                //};

                // hangfire
                //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
                //isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

                if (!isSent)
                {
                    // send warning
                }

                response.Message = _localizer["SuccessMsg"].Value;
                response.Code = Generalcodes.Ok.ToString();
                response.Success = true;

                return Ok(response);
            }

            var transferResult = await _walletService.TransferBalanceAsync(new TransferBalanceRequest
            {
                Token = driver.Token,
                MobileNumber = rider.MobileNumber,
                TransferBalance = refillRequest.Amount,
                TransferCurrency = "ILS",
            });
            // insert the record in the DB
            if (transferResult.Success)
            {
                await _userTransactionsService.AddUserTransactionAsync(new UserTransactions
                {
                    CreationDate = DateTime.Now,
                    FromUserId = driver.UserId,
                    //FromUserId = sender.UserId,
                    ToUserId = rider.UserId,
                    //ToUserId = receiver.UserId,
                    Type = UserTransactionsTypesEnum.Transfer.ToString(),
                    Amount = refillRequest.Amount,
                    UserTransactionTypeId = (int)UserTransactionsTypesEnum.Transfer
                });

                await _refillRequestsService.UpdateRefillRequest(new RefillRequest
                {
                    RiderId = request.RiderId,
                    DriverId = driverId,
                    Amount = request.Amount,
                    IsHandled = true,
                    IsSuccess = true
                });
                //response.Success = true; ;
                //response.Message = "تم معالجة طلبك بنجاح";
                //response.Code = Generalcodes.Ok.ToString();
            }
            else
            {
                response.Message = Request.Headers?["Content-Language"].ToString() == "ar" ? transferResult.MessageAr : transferResult.MessageEn;

                await _refillRequestsService.UpdateRefillRequest(new RefillRequest
                {
                    RiderId = request.RiderId,
                    DriverId = driverId,
                    Amount = request.Amount,
                    IsHandled = true,
                    IsSuccess = false
                });

                lang = rider.User.Language;
                title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationMsgTitle_ar :
                       NotificationTexts.WalletNotification.RefillNotificationMsgTitle_en;
                body = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationFailureResponseMsgBody_ar :
                   NotificationTexts.WalletNotification.RefillNotificationFailureResponseMsgBody_en;


                isSent = await _notificationService.SendNotificationAsync(rider.User.FCMToken
          , body, title, "", "RefillResponse",
          "", RolesEnum.Rider);

                //pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = title,
                //        body = body,
                //        category = "RefillResponse",
                //    },
                //    notification = new Notification
                //    {
                //        title = title,
                //        body = body,
                //        category = "RefillResponse",
                //    },
                //    to = rider.User.FCMToken
                //};

                return UnprocessableEntity(response);
            }

            lang = rider.User.Language;
            title = lang == "ar" ? NotificationTexts.WalletNotification.RefillNotificationMsgTitle_ar :
              NotificationTexts.WalletNotification.RefillNotificationMsgTitle_en;
            body = lang == "ar" ?
            string.Format(NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgBody_ar, request.Amount) :
            string.Format(NotificationTexts.WalletNotification.RefillNotificationSuccessResponseMsgBody_en,
            request.Amount);


            isSent = await _notificationService.SendNotificationAsync(rider.User.FCMToken
             , body, title, "", "RefillResponse",
             "", RolesEnum.Rider);

            //pushNotificationDto = new PushNotificationDto()
            //{
            //    data = new Data
            //    {
            //        title = title,
            //        body = body,
            //        category = "RefillResponse",
            //    },
            //    notification = new Notification
            //    {
            //        title = title,
            //        body = body,
            //        category = "RefillResponse",
            //    },
            //    to = rider.User.FCMToken
            //};

            // hangfire
            //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
            //isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

            if (!isSent)
            {
                // send warning
            }

            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;
            return Ok(response);
        }



    }
}
