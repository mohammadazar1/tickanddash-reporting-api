
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Filters;
using TickAndDash.HttpClients.GeoClients.DTOs;
using TickAndDash.HttpClients.GeoClients.Interfaces;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class RidersQueueController : ControllerBase
    {
        private readonly ICarsTripsService _carsTripsService;
        private readonly ICarService _carService;
        private readonly IPickupStationsService _pickupStationsService;
        private readonly IMapper _mapper;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly ITripsService _tripsService;
        private readonly ICarsQueueService _carsQueueService;
        private readonly IRidersQueueService _ridersQueueService;
        private readonly IStringLocalizer<RidersQueueController> _localizer;
        private readonly IRidersTicketsServices _ridersTicketsService;
        private readonly IDigitalCodexClient _digitalCodexClient;
        private readonly IUsersService _usersService;
        private readonly ITransItinerariesService _transItinerariesService;
        private readonly IUserTransactionsService _userTransactionsService;
        private readonly INotificationService _notificationService;
        private ILogger<RidersQueueController> _logger;
        public RidersQueueController(IPickupStationsService pickupStationsService,
            IMapper mapper, ISystemConfigurationService systemConfigurationService,
            ITripsService tripsService, ICarsQueueService carsQueueService, IRidersQueueService ridersQueueService,
            IStringLocalizer<RidersQueueController> localizer,
            IRidersTicketsServices ridersTicketsService, IDigitalCodexClient digitalCodexClient, IUsersService usersService,
            ITransItinerariesService transItinerariesService, IUserTransactionsService userTransactionsService,
            INotificationService notificationService, ICarsTripsService carsTripsService,
            ILogger<RidersQueueController> logger,
            ICarService carService
            )
        {
            _pickupStationsService = pickupStationsService;
            _mapper = mapper;
            _systemConfigurationService = systemConfigurationService;
            _tripsService = tripsService;
            _carsQueueService = carsQueueService;
            _ridersQueueService = ridersQueueService;
            _localizer = localizer;
            _ridersTicketsService = ridersTicketsService;
            _digitalCodexClient = digitalCodexClient;
            _usersService = usersService;
            _transItinerariesService = transItinerariesService;
            _userTransactionsService = userTransactionsService;
            _notificationService = notificationService;
            _carsTripsService = carsTripsService;
            _logger = logger;
            _carService = carService;
        }

        /// <summary>
        /// Request for the rider to book several seats in any available pickup station
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **FMC Category = "Booking"** <br/>
        /// **Codes:**<br/>
        /// 1.PickUp_1 pickup not active for this driver<br/>
        /// 2.Date_1 datetime is not valid <br/>
        /// 3.Data_1 No mathing data <br/>
        /// 4.Param_1 BadRequest by missing paramter request or passing unvalid value. <br/>
        /// 5.PickUp_2  has an active reservation today <br/>
        /// 6.sql_1 Database failure 
        /// **Limits** 
        /// The Rider can book only -N-15 minutes in advance <br/>
        /// </remarks>
        /// <response code = "200"> Success </response>
        /// <response code = "400"> Failed </response> 
        /// <response code = "403"> Failed </response> 
        /// <response code = "404"> Failed </response> 
        [HttpPost("Booking")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [ProducesResponseType(typeof(GeneralResponse<PushRidersToQueueResponse>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> Booking(PushRidersToQueueRequest request)
        {

            GeneralResponse<PushRidersToQueueResponse> response = new GeneralResponse<PushRidersToQueueResponse>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = null,
                Code = ""
            };

            string _language = Request.Headers["Content-Language"];

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            string mobileNumber = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            var rider = await _usersService.GetRiderByMobileNumberAsync(mobileNumber);
            decimal price = await _transItinerariesService.
                GetTransItineraryPriceByPickupStation(request.PickupStationId);

            //if ((DateTime.Now - rider.SubscriptionDate).TotalDays > 30 &&
            //    rider.LastBillingDate != null
            //    && (DateTime.Now - rider.LastBillingDate).TotalDays < 30
            //    )
            //{
            //    price += 4;
            //    //rider.LastBillingDate = DateTime.Now;
            //    //await _usersService.UpdateRiderAsync(rider);
            //}
            int countOfSeatLimit = int.Parse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.SeatsCount));
            if (request.CountOfSeats > countOfSeatLimit)
            {
                response.Message = _localizer["SeatLimitError"].Value; /*"عذرًا، لقد تجاوزت الحد المسموح به لعدد المقاعد";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return StatusCode(400, response);
            }

            bool isDateFormateValid = DateTime.TryParse(request.ReservationDate, /*new CultureInfo("en"), System.Globalization.DateTimeStyles.None,*/
                out DateTime reservationDate);


            if (!isDateFormateValid)
            {
                response.Message = _localizer["NotValidDateError"].Value; /*"عذرًا، يرجى ادخال الموعد بالتنسيق المقترح";*/
                response.Code = ValidationCodes.Date_1.ToString();
                return StatusCode(400, response);
            }

            int reserveTimeLimit = int.Parse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.ReserveTimeLimit));
            int timeLimitToCancellReservation = int.Parse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation));

            if ((reservationDate - DateTime.Now).TotalMinutes < reserveTimeLimit
                || (reservationDate - DateTime.Now).TotalHours > timeLimitToCancellReservation
                )
            {
                response.Message = _localizer["ReservationTimeError"].Value; /*"عذرًا، يرجى اختيار وقت متاح للحجز";*/
                response.Code = ValidationCodes.Date_1.ToString();
                return StatusCode(400, response);
            }

            bool isPickupActive = await _pickupStationsService.IsPickupStationValidAndActiveAsync(request.PickupStationId);

            if (!isPickupActive)
            {
                response.Message = _localizer["NotValidPickup"].Value; /*"عذرًا، لايمكنك حجز دور في هذا الموقف";*/
                response.Code = ValidationCodes.PickUp_1.ToString();
                return StatusCode(422, response);
            }
            // replaced with is Rider in queue, inQueue = false
            //bool isWaiting = _ridersQueueService.IsRiderInQueueWithStatus(userId, request.PickupStationId, RidersQStatusLookupEnum.Waiting);
            //bool isInQueue = await _ridersQueueService.IsRiderInAnyQueueAsync(userId);
            var riderReservation = await _ridersQueueService.GetRiderActiveReservationAsync(userId);
            if (riderReservation != null)
            {
                response.Message = _localizer["ActiveReservationError"].Value; /*"عذرًا، لديك حجز فعال";*/
                response.Code = ValidationCodes.PickUp_2.ToString();
                return StatusCode(400, response);
            }
            //bool isCanceled = _queuesService.CancelAllExpiredReservation(userId);
            //if (!isCanceled)
            //{
            //    response.Message = "";
            //    response.Code = UnExpectedErrors.sql_1.ToString();
            //    return UnprocessableEntity(response);
            //}
            /*   var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
               clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;
               Thread.CurrentThread.CurrentCulture = clone;*/

            var resrveResponse = await _digitalCodexClient.ReserveBalanceAsync(new ReserveBalanceRequest
            {
                ReservationBalance = price * request.CountOfSeats,
                ReservationCurrency = "ILS",
                ReservationPeriod = 5,
                Token = rider.Token
            });

            if (resrveResponse.Success == false && resrveResponse.Code != "ACTIVE")
            {

                if (resrveResponse.Code == "NoBalance" || resrveResponse.MessageEn.Contains("balance"))
                {
                    //عذراً ليس لديك رصيد كافٍ لحجز المقعد، يرجى تعبئة الرصيد.
                    response.Message = _localizer["NoBalanceMSg"].Value;
                    response.Success = true;
                    return UnprocessableEntity(response);
                }

                response.Message = _language == "ar" ? resrveResponse.MessageAr : resrveResponse.MessageEn;
                return UnprocessableEntity(response);
            }


            DateTime reservationcreationDate = DateTime.Now;
            int riderQid = await _ridersQueueService.AddToRidersQueueAsync(userId,
                 request.PickupStationId,
                 RidersQStatusLookupEnum.Waiting,
                 reservationcreationDate, 
                 reservationDate,
                 request.CountOfSeats
                );

            // Check if there is active reservation for rider
            if (riderQid < 1)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا حدث خطأ ما يرجى المحاولة مجددًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return UnprocessableEntity(response);
            }

            int countOfReservationBeforeMe = await _ridersQueueService.GetCountOfReservationBeforeMeAsync(reservationDate,
                reservationcreationDate, request.PickupStationId, riderQid);

            //response.Data.RidersInQ = _queuesService.GetCountOfRidersInFrontOfMyTurn(reservationDate, request.PickupStationId);
            response.Data = new PushRidersToQueueResponse
            {
                Turn = countOfReservationBeforeMe + 1
                //Turn = countOfReservationBeforeMe
            };
            //string FCMToken = _carsQueueService.GetActiveDriverInPickupQ(request.PickupStationId)?.User?.FCMToken;
            //int reservationLimit = int.Parse(_systemConfigurationService.GetSettingValueByKey(SettingKeyEnum.ReserveTimeLimit));
            //if (response.Data.Turn < 8 && !string.IsNullOrEmpty(FCMToken) && reservationDate < DateTime.Now.AddMinutes(reservationLimit))
            //{
            //    //string FCM = User.FindFirstValue(ClaimsEnum.FCM.ToString());
            //    PushNotificationDto pushNotificationDto = new PushNotificationDto()
            //    {
            //        data = new Data
            //        {
            //            title = _localizer["NewRiderNotificationTitle"].Value,/* "قائمة الركّاب",*/
            //            body = _localizer["NewRiderNotification"].Value, /*"راكب جديد على قائمة الركّاب",*/
            //            category = "Booking"
            //        },
            //        notification = new Notification
            //        {
            //            title = _localizer["NewRiderNotificationTitle"].Value,/*"قائمة الركّاب",*/
            //            body = _localizer["NewRiderNotification"].Value, /*"راكب جديد على قائمة الركّاب",*/
            //            category = "Booking"
            //        },
            //        to = FCMToken
            //    };
            //    // hangfire
            //    //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
            //    bool isSent = await _fCMHttpClient.PushNotifications(pushNotificationDto);

            //    if (!isSent)
            //    {
            //        // send warning
            //    }
            //}

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        ///<summary>
        ///Request for Riders to cancell the booking
        ///</summary>
        ///<returns></returns>
        [HttpPost("Cancellation")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> CancelReservation()
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = null,
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            bool isCancellationValid = await _systemConfigurationService.IsCancellingValidForRiderAsync(userId);

            if (!isCancellationValid)
            {
                response.Message = _localizer["CancellationError"].Value; /*"عذرًا، لايمكنك الغاء الحجز!";*/
                response.Code = ValidationCodes.Cancel_1.ToString();
                return BadRequest(response);
            }

            // Check if there is active reservation for rider
            var riderInQueue = await _ridersQueueService.GetActiveRidersInQWithStatusAsync(userId, RidersQStatusLookupEnum.Waiting);

            if (riderInQueue is null /*|| riderInQueue.ReservationDate < DateTime.Now*/)
            {
                response.Message = _localizer["NoActiveReservationError"].Value; /* "عذرًا، ليس لديك حجز فعال حاليًا";*/
                response.Code = ValidationCodes.PickUp_2.ToString();
                return BadRequest(response);
            }

            bool isUpdated = await _ridersQueueService.UpdateRiderStatusInQueueAsync(riderInQueue.Id, RidersQStatusLookupEnum.Canceled);
            if (!isUpdated)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /* "عذرًا حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return UnprocessableEntity(response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;/*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }
        /// <summary>
        /// Request for Riders to skip their car
        /// </summary>
        /// <returns></returns>
        /// <returns></returns>       
        [HttpPost("Skiping")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> RiderSkipTurn(RiderSkipCountRequest request)
        {
            GeneralResponse<RiderSkipCountResponse> response = new GeneralResponse<RiderSkipCountResponse>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = null
            };

            // Count Of Skip time 
            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            //var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            //clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;
            //Thread.CurrentThread.CurrentCulture = clone;


            var riderQueue = await _ridersQueueService.GetActiveRidersInQWithStatusAsync(userId, RidersQStatusLookupEnum.notified);

            if (riderQueue is null)
            {
                response.Message = _localizer["NoActiveReservationError"].Value; /* "عذرًا، ليس لديك حجز فعال حاليًا";*/
                response.Code = Generalcodes.Ok.ToString();
                return BadRequest(response);
            }


            if (riderQueue.SkipCount > 0)
            {
                response.Message = _localizer["AutomaticSkipMsg"].Value; /* "عذرًا، ليس لديك حجز فعال حاليًا";*/
                response.Code = Generalcodes.Ok.ToString();
                return BadRequest(response);
            }

            //int countOfCars = _ridersQueueService.GetCountOfRidersAfterMyTurn(userId, riderQueue.PickupStationId);
            //if (countOfCars < request.SkipCount)
            //{
            //    response.Message = _localizer["SkipError", request.SkipCount].Value;  /*$"عذرًا، لايتواجد {0} ركاب لديهم حجز فعالك خلفك";*/
            //    response.Code = QueusCode.car_2.ToString();
            //    return UnprocessableEntity(response);
            //}
            var driver = await _carsQueueService.GetActiveDriverInPickupQAsync(riderQueue.PickupStationId);

            if (driver == null)
            {
                response.Message = _localizer["NoDriverInTheQ"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return UnprocessableEntity(response);
            }

            TimeSpan time = TimeSpan.Parse(request.SkipTime);

            var reservationDate = riderQueue.ReservationDate.Add(time);
            bool isSkiped = await _ridersQueueService.UpdateRiderSkipTimeAsync(riderQueue.Id, time, reservationDate);

            if (!isSkiped)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value;/* "عذرً، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return UnprocessableEntity(response);
            }

            //if (driver != null)
            //{
            //    //PushNotificationDto pushNotificationDto = new PushNotificationDto()
            //    //{
            //    //    data = new Data
            //    //    {
            //    //        title = _localizer["SkipDriverNotificationTitle"].Value,
            //    //        body = _localizer["SkipDriverNotification"].Value,// "تم دفع المبلغ بنجاح",
            //    //        category = "Skipped"
            //    //    },
            //    //    notification = new Notification
            //    //    {
            //    //        //title = "الأجرة",
            //    //        //body = "تم دفع المبلغ بنجاح",
            //    //        title = _localizer["SkipDriverNotificationTitle"].Value,
            //    //        body = _localizer["SkipDriverNotification"].Value,
            //    //        category = "Skipped"
            //    //    },
            //    //    to = driver.User.FCMToken
            //    //};
            //    string lang = driver.User.Language;
            //    string title = lang == "ar" ? NotificationTexts.RidersQNotification.SkipDriverNotificationTitle_ar :
            //            NotificationTexts.RidersQNotification.SkipDriverNotificationTitle_en;
            //    string body = lang == "ar" ? NotificationTexts.RidersQNotification.SkipDriverNotificationBody_ar :
            //            NotificationTexts.RidersQNotification.SkipDriverNotificationBody_en;
            //    bool isSent = await _notificationService.SendNotificationAsync(driver.User.FCMToken
            //          , body, title, "", "Skipped",
            //          driver.MobileOS, RolesEnum.Driver);
            //    //PushNotificationDto pushNotificationDto = new PushNotificationDto()
            //    //{
            //    //    data = new Data
            //    //    {
            //    //        title = title,
            //    //        body = body,
            //    //        category = "Skipped"
            //    //    },
            //    //    notification = new Notification
            //    //    {
            //    //        title = title,
            //    //        body = body,
            //    //        category = "Skipped"
            //    //    },
            //    //    to = driver.User.FCMToken
            //    //};
            //    // hangfire
            //    //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
            //    //bool isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);
            //    if (!isSent)
            //    {
            //        // send warning
            //    }
            //}

            response.Data = new RiderSkipCountResponse()
            {
                ReservationDate = reservationDate
            };

            response.Message = _localizer["SuccessMsg"].Value; ;/*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;

            return Ok(response);
        }

        //ADD  USER TO THE NEW TABLE 
        // if there is no fee bill him out or what 
        /// <summary>
        /// Request for Riders to confirm his reservation after receving the push notfication from the driver
        /// </summary>
        /// <returns></returns>

        [HttpPost("Accepting")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> AcceptDriverRequest()
        {
            GeneralResponse<AcceptDriverResponse> response = new GeneralResponse<AcceptDriverResponse>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = new AcceptDriverResponse(),
                Success = true,
                Code = ""
            };
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            // Check notification time

            var acceptedRider = await _ridersQueueService.GetRiderInAnyQueueWithStatusAsync(userId, RidersQStatusLookupEnum.Accepted);

            if (acceptedRider != null)
            {
                response.Success = true;
                response.Message = _localizer["AlreadyAccepted"].Value;
                response.Data.Ticket = await _ridersTicketsService.GetRiderTicketAsync(acceptedRider.Id);
                return BadRequest(response);
            }

            var ridersQueue = await _ridersQueueService.GetActiveRidersInQWithStatusAsync(userId, RidersQStatusLookupEnum.notified);
            //if (ridersQueue == null)
            //{
            //    response.Message = _localizer["NoActiveReservationError"].Value; /*"الحافلة ليس لها دور مسجل!";*/
            //    response.Code = QueusCode.car_2.ToString();
            //    return NotFound(response);
            //}

            if (ridersQueue == null || ridersQueue?.PickupStationId < 1)
            {
                response.Message = _localizer["NoActiveReservationError"].Value; /*"عذرًا، لايوجد لديك حجز فعال!";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return BadRequest(response);
            }
            // billing Section init fee

            var driver = await _carsQueueService.GetActiveDriverInPickupQAsync(ridersQueue.PickupStationId);
            if (driver == null)
            {
                response.Message = _localizer["NoDriverInTheQ"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return UnprocessableEntity(response);
            }

            bool isUpdated = await _ridersQueueService.UpdateRiderStatusInQueueAsync(ridersQueue.Id, RidersQStatusLookupEnum.Accepted);
            if (!isUpdated)
            {
                // log warning
            }

            //_queuesService.GetRaiderDailyCancellationCount()

            if (driver != null)
            {
                string lang = driver.User.Language;
                string title = lang == "ar" ? NotificationTexts.RidersQNotification.AcceptDriverNotificationTitle_ar :
                        NotificationTexts.RidersQNotification.AcceptDriverNotificationTitle_en;
                string body = lang == "ar" ? NotificationTexts.RidersQNotification.AcceptDriverNotificationBody_ar :
                        NotificationTexts.RidersQNotification.AcceptDriverNotificationBody_en;

                //PushNotificationDto pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = _localizer["AcceptDriverNotificationTitle"].Value,
                //        body = _localizer["AcceptDriverNotification"].Value,/* "تم تأكيد الحجز بنجاح",*/
                //        category = "Accepted"
                //    },
                //    notification = new Notification
                //    {
                //        title = _localizer["AcceptDriverNotificationTitle"].Value, /*"تأكيد الحجز",*/
                //        body = _localizer["AcceptDriverNotification"].Value, /*"تم تأكيد الحجز بنجاح",*/
                //        category = "Accepted"
                //    },
                //    to = driver.User.FCMToken
                //};

                bool isSent = await _notificationService.SendNotificationAsync(driver.User.FCMToken
                      , body, title, "", "Accepted",
                      driver.MobileOS, RolesEnum.Driver);
                //PushNotificationDto pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = title,
                //        body = body,/* "تم تأكيد الحجز بنجاح",*/
                //        category = "Accepted"
                //    },
                //    notification = new Notification
                //    {
                //        title = title, /*"تأكيد الحجز",*/
                //        body = body, /*"تم تأكيد الحجز بنجاح",*/
                //        category = "Accepted"
                //    },
                //    to = driver.User.FCMToken
                //};
                // hangfire
                //Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
                //isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

                if (!isSent)
                {
                    // send warning
                }
            }

            RidersTickets ridersTickets = new RidersTickets()
            {
                RiderQId = ridersQueue.Id,
                Ticket = $"{driver?.Car?.CarCode}_{userId}"
            };

            int Tid = await _ridersTicketsService.AddRidersTickAsync(ridersTickets);
            ridersTickets.Ticket += $"_{Tid}";

            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            response.Data.Ticket = ridersTickets.Ticket;
            response.Success = true;

            return Ok(response);
        }

        /// <summary>
        /// Request for driver to confirm the billing and to pull rider form the queue
        /// </summary>
        /// <returns></returns>
        [HttpPost("Billing/deduct")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> ConfirmDriverRequest([FromBody] ConfirmRidersRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            string mobileNumber = User.FindFirstValue(ClaimsEnum.MobileNumber.ToString());
            int userid = int.Parse(User.FindFirstValue(ClaimsEnum.UserId.ToString()));

            var carQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carQueue == null)
            {
                response.Message = _localizer["NoActiveReservationError"].Value; /*"الحافلة ليس لها دور مسجل!";*/
                response.Code = QueusCode.car_2.ToString();
                return NotFound(response);
            }

            var riderQ = await _ridersQueueService.GetRidersInQWithStatusAsync(request.RiderQId, RidersQStatusLookupEnum.Accepted);
            //var carTurn = _queuesService.GetCarTurnInPPickup(carQueue.PickupStationId);
            //if (carTurn.CarId != carId)
            //{
            //    response.Message = "عذرًا، يوجد سائقين أمامك في الدور";
            //    response.Code = QueusCode.car_3.ToString();
            //    return StatusCode(422, response);
            //}
            //bool isInQueue = _queuesService.IsRiderInQueueWithStatus(request.RiderQId, RidersQStatusLookupEnum.Accepted);
            //var riderQ = _queuesService.GetRidersInQ(request.RiderQId, RidersQStatusLookupEnum.Accepted);
            if (riderQ == null || !riderQ.IsInQueue)
            {
                response.Message = _localizer["AcceptingError"].Value;/* "لم يقم الراكب بتاكيد الطلب";*/
                response.Code = QueusCode.rid_1.ToString();
                return UnprocessableEntity(response);
            }

            var rider = await _usersService.GetRiderByIdAsync(riderQ.RiderId);
            decimal price = await _transItinerariesService.GetTransItineraryPriceByPickupStation(riderQ.PickupStationId);
            //if ((DateTime.Now - rider.SubscriptionDate).TotalDays > 30 &&
            //    rider.ActiveSubscriptionPeriod != null
            //    && (DateTime.Now - rider.ActiveSubscriptionPeriod).TotalDays < 30
            //    )
            //{
            //    price += 4;
            //    //rider.LastBillingDate = DateTime.Now;
            //    //await _usersService.UpdateRiderAsync(rider);
            //}
            var transferResponse = await _digitalCodexClient.TransferBalanceAsync(new HttpClients.GeoClients.DTOs.TransferBalanceRequest
            {
                MobileNumber = mobileNumber,
                TransferBalance = price * riderQ.CountOfSeats,
                TransferCurrency = "ILS",
                Username = "",
                Token = rider.Token
            });

            if (transferResponse.Success == false)
            {
                //bool isUpdated = await _ridersQueueService.
                //    UpdateRiderStatusInQueueAsync(request.RiderQId, RidersQStatusLookupEnum.Confirmed);


                //if (!isUpdated)
                //{
                return UnprocessableEntity(response);
                //}
            }
            // billing Section init fee
            //bool isSkiped = _queuesService.UpdateRiderStatusInQueueByRiderqId(request.RiderQId, RidersQStatusLookupEnum.Confirmed);

            await _userTransactionsService.AddUserTransactionAsync(new UserTransactions
            {
                CreationDate = DateTime.Now,
                FromUserId = riderQ.RiderId,
                ToUserId = userid,
                Type = UserTransactionsTypesEnum.TripBilling.ToString(),
                Amount = price * riderQ.CountOfSeats,
                UserTransactionTypeId = (int)UserTransactionsTypesEnum.TripBilling
            });


            bool isSkiped = await _ridersQueueService.UpdateRiderStatusInQueueAsync(request.RiderQId, RidersQStatusLookupEnum.Confirmed);

            if (riderQ != null)
            {
                string lang = riderQ.User.Language;
                string title = lang == "ar" ? NotificationTexts.RidersQNotification.PayingNotificationTitle_ar :
                        NotificationTexts.RidersQNotification.PayingNotificationTitle_en;

                string body = lang == "ar" ?
                    string.Format(NotificationTexts.RidersQNotification.PayingNotificationBody_ar, price * riderQ.CountOfSeats) :
                      string.Format(NotificationTexts.RidersQNotification.PayingNotificationBody_en, price * riderQ.CountOfSeats);

                ////await _transItinerariesService.GetTransItineraryPriceByPickupStation(riderQ.PickupStationId);
                //PushNotificationDto pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = _localizer["PayingNotificationTitle"].Value,
                //        body = _localizer["PayingNotificationBody", price * riderQ.CountOfSeats].Value,// "تم دفع المبلغ بنجاح",
                //        category = "Billing"
                //    },
                //    notification = new Notification
                //    {
                //        //title = "الأجرة",
                //        //body = "تم دفع المبلغ بنجاح",
                //        title = _localizer["PayingNotificationTitle"].Value,
                //        body = _localizer["PayingNotificationBody", price * riderQ.CountOfSeats].Value,
                //        category = "Billing"
                //    },
                //    to = riderQ.User.FCMToken
                //}; 


                bool isSent = await _notificationService.SendNotificationAsync(riderQ.User.FCMToken
                      , body, title, "", "Billing",
                      "", RolesEnum.Rider);

                //PushNotificationDto pushNotificationDto = new PushNotificationDto()
                //{
                //    data = new Data
                //    {
                //        title = title,
                //        body = body,// "تم دفع المبلغ بنجاح",
                //        category = "Billing"
                //    },
                //    notification = new Notification
                //    {
                //        //title = "الأجرة",
                //        //body = "تم دفع المبلغ بنجاح",
                //        title = title,
                //        body = body,
                //        category = "Billing"
                //    },
                //    to = riderQ.User.FCMToken
                //};

                //// hangfire
                ////Task.Run( async () => _fCMHttpClient.PushNotifications(pushNotificationDto));
                //bool isSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

                if (!isSent)
                {
                    // send warning
                }
            }

            var carTripId = await _tripsService.GetCarTripByCarsQueueIDAsync(carQueue.Id);

            if (carTripId > 0)
            {
                bool isAdded = await _tripsService.AddToTripRiderAsync(riderQ.Id, riderQ.User.Id, carTripId);

                if (!isAdded)
                {
                    // log warning
                }
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// This is the QR code API for confirming the presence of the rider inside the car
        /// used by rider app 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("Confirm/presence")]
        [Authorize(Roles = "Rider")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> ConfirmRiderPresence(ConfirmRiderPresenceRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Code = "",
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            int carId = await _carService.GetCarIdByCarCodeAsync(request.CarCode);

            await _carService.GetCarActiveDriverIdByCarIdAsync(carId);

            var driver = await _usersService.GetDriverByCarIdAsync(carId);

            if (carId < 1)
            {
                response.Success = false;
                response.Message = _localizer["WrongData"].Value;
                return BadRequest(response);
            }

            var carQ = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            //var driver = await _carsQueueService.GetActiveDriverInPickupQAsync(carQ.PickupStationId);
            if (carQ == null)
            {
                int riderQId = await _carsTripsService.GetRiderQIdFromLastCarTripAsync(request.CarCode, userId);

                if (riderQId < 0)
                {
                    response.Message = _localizer["QRTripRiderNotRecognized"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                    response.Code = "";
                    return BadRequest(response);
                }

                if (await _ridersQueueService.GetRiderPresenceStatusAsync(riderQId))
                {
                    response.Message = _localizer["QRTripRiderAlreadyPresent"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                    response.Code = "";
                    return BadRequest(response);
                }

                await _ridersQueueService.UpdateRiderPresenceStatusAsync(riderQId);
            }
            else if (carQ != null && carQ.Turn == 1)
            {
                var riderQ = await _ridersQueueService.GetRiderInAnyQueueWithStatusAsync(userId, RidersQStatusLookupEnum.Accepted) ??
                    await _ridersQueueService.GetRiderInAnyQueueWithStatusAsync(userId, RidersQStatusLookupEnum.Confirmed);


                int psId = await _pickupStationsService.GetMinorPickupStationMainStaionId(riderQ.PickupStationId);
                riderQ.PickupStationId = psId == 0 ? riderQ.PickupStationId : psId;
                List<int> pickups = new List<int>();
                pickups.Add(riderQ.PickupStationId);

                var minorPickups = await _pickupStationsService.GetMinorPickupStationsThatFollowsMainPickupStationAsync(riderQ.PickupStationId, "ar");

                if (minorPickups != null || !minorPickups.Any())
                {
                    var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                    pickups.AddRange(minorPs);
                }

                ;

                if (riderQ == null || !pickups.Contains(carQ.PickupStationId))// riderQ.PickupStationId != carQ.PickupStationId)
                {
                    response.Message = _localizer["QRTripRiderNotRecognized"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                    response.Code = "";
                    return BadRequest(response);
                }

                if (await _ridersQueueService.GetRiderPresenceStatusAsync(riderQ.Id))
                {
                    response.Message = _localizer["QRTripRiderAlreadyPresent"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                    response.Code = "";
                    return BadRequest(response);
                }

                await _ridersQueueService.UpdateRiderPresenceStatusAsync(riderQ.Id);
            }
            else
            {
                response.Message = _localizer["QRTripRiderNotRecognized"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = "";
                return BadRequest(response);
            }

            if (driver != null)
            {
                string lang = driver.User.Language;
                string title = lang == "ar" ? NotificationTexts.RidersQNotification.AttendanceNotificationTitle_ar :
                        NotificationTexts.RidersQNotification.AttendanceNotificationTitle_en;

                string body = lang == "ar" ? NotificationTexts.RidersQNotification.AttendanceNotificationBody_ar :
                        NotificationTexts.RidersQNotification.AttendanceNotificationBody_en;

                bool isSent = await _notificationService.SendNotificationAsync(driver.User.FCMToken
                 , body, title, "", "Attendance",
                 driver.MobileOS, RolesEnum.Driver);

                if (!isSent)
                {

                }
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// Request for Drivers to Get all riders in his pickup station Q with waiting status
        /// Not Used AnyMore
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Driver")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetAllRidersInQ()
        {
            throw new NotImplementedException();

            string _language = Request.Headers["Content-Language"];

            GeneralResponse<List<RidersQResponse>> response = new GeneralResponse<List<RidersQResponse>>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = new List<RidersQResponse>()
            };
            // Should take ps Id from the Qid that driver is on it

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);

            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carsQueue == null)
            {
                response.Message = _localizer["NoTurn"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            if (carsQueue.Turn != 1)
            {
                response.Message = _localizer["NotYourTurn"].Value;  /* "عذرًا، الدور ليس دورك الآن";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return UnprocessableEntity(response);
            }


            //var ridersQ = _queuesService.GetAllRidersInQ(RidersQStatusLookupEnum.Waiting, car.PickupStationId);
            // Get Seat View Rider

            var seatViewRiders = await _ridersQueueService.
                GetActiveSeatViewRidersReservationInPickupStationAsync(carsQueue.PickupStationId, _language);

            int seatViewCountOfSeats = 0;

            foreach (var seatView in seatViewRiders)
            {
                seatViewCountOfSeats += seatView.CountOfSeats;
            }


            //int seatViewRidersCount = seatViewRiders.Count();

            if (seatViewCountOfSeats < carsQueue.Car.SeatCount)
            {
                var ridersQ = await _ridersQueueService.
                    GetFirstNRidersSeatsCountInPickupStationQAsync(carsQueue.PickupStationId, carsQueue.Car.SeatCount - seatViewCountOfSeats);

                if (ridersQ == null || !ridersQ.Any())
                {
                    response.Message = _localizer["NoReservations"].Value; /* "عذرًا، لايوجد حجوزات حاليًا";*/
                    response.Code = ValidationCodes.Data_1.ToString();
                    return NotFound(response);
                }

                var riders = _mapper.Map<List<RidersQResponse>>(ridersQ);
                response.Data = riders;
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        // Get only the last 15 minutes or such thing 
        /// <summary>
        /// Request for Drivers in the turn to get his last trip seat view 
        /// </summary>
        /// <returns></returns>
        [HttpGet("SeatsView")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Driver")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        //[TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetSeatViewForDriverInQ()
        {
            GeneralResponse<SeatViewResponse> response = new GeneralResponse<SeatViewResponse>()
            {
                Success = true,
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = new SeatViewResponse()
            };
            string _language = Request.Headers["Content-Language"];


            // Should take ps Id from the Qid that driver is on it
            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            int driverId = await _carService.GetCarActiveDriverIdByCarIdAsync(carId);

            if (driverId == 0)
            {
                response.Success = false;
                response.Message = _localizer["NoDriver"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return UnprocessableEntity(response);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            if (driverId != userId)
            {
                response.Success = false;
                response.Message = _localizer["AnotherDriverActive"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = Generalcodes.Auth_2.ToString();
                return UnprocessableEntity(response);
            };

            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carsQueue == null || carsQueue.Turn != 1)
            {
                var lastCarTrip = await _carsTripsService.GetLastCarTripInfoAsync(carId);

                if (lastCarTrip == null || !lastCarTrip.Any())
                {
                    response.Success = true;
                    response.Message = _localizer["NoTrips"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                    response.Code = ValidationCodes.Data_1.ToString();

                    return NotFound(response);
                }

                var ridersQResponses = _mapper.Map<List<RidersQResponse>>(lastCarTrip);
                response.Data.RidersQResponse = ridersQResponses;
                response.Data.IsTripActive = false;

                response.Success = true;
                response.Message = _localizer["SuccessMsg"].Value;
                response.Code = Generalcodes.Ok.ToString();

                return Ok(response);
            }

            //var activeCarTurnId = await _carsQueueService.GetActiveDriverInPickupQAsync(carsQueue.PickupStationId);
            //if (carId != activeCarTurnId.CarId)
            //{
            //    response.Message = _localizer["WaitYourTurn"].Value; /* "عليك انتظار دورك لتفعيل هذه العملية";*/
            //    response.Code = ValidationCodes.Data_1.ToString();
            //    response.Success = true;
            //    return UnprocessableEntity(response);
            //}

            //var ridersQ = _queuesService.GetAllRidersInQ(RidersQStatusLookupEnum.Waiting, car.PickupStationId);
            var ridersQ = await _ridersQueueService.GetActiveSeatViewRidersReservationInPickupStationAsync(carsQueue.PickupStationId, _language);

            if (ridersQ == null || !ridersQ.Any())
            {
                response.Message = _localizer["NoReservations"].Value; /*"عذرًا، لاتوجد مقاعد محجوزة حاليًا";*/
                response.Code = ValidationCodes.Data_1.ToString();
                response.Data.IsTripActive = true;
                response.Success = true;
                response.Data.RidersQResponse = new List<RidersQResponse>();
                return NotFound(response);
            }
            //List<StationsBooking> stationsBookings = new List<StationsBooking>();
            //foreach (var rider in ridersQ)
            //{
            //    var station = stationsBookings.Find(x => x.Name == rider.PickupStations.Sites.Name);
            //    if (station != null)
            //    {
            //        station.count += rider.CountOfSeats;
            //    }
            //    else
            //    {
            //        station = new StationsBooking
            //        {
            //            Name = rider.PickupStations.Sites.Name,
            //            count = rider.CountOfSeats
            //        };
            //        stationsBookings.Add(station);
            //    }
            //    //rider.PickupStations.Sites.Name;
            //}
            foreach (var r in ridersQ)
            {
                if (r.RidersQStatusLookupId == (int)RidersQStatusLookupEnum.Ticket)
                {
                    int tripId = await _tripsService.GetCarTripByCarsQueueIDAsync(carsQueue.Id);

                    if (!await _tripsService.IsRiderQTripExistAsync(r.Id))
                    {
                        var driver = await _carsQueueService.
                            GetActiveDriverInPickupQAsync(carsQueue.PickupStationId);

                        RidersTickets ridersTickets = new RidersTickets()
                        {
                            RiderQId = r.Id,
                            Ticket = $"{driver?.Car?.CarCode}_{r.User.Id}"
                        };

                        await _tripsService.AddToTripRiderAsync(r.Id, r.User.Id, tripId);
                    }
                }
            }

            var riders = _mapper.Map<List<RidersQResponse>>(ridersQ);
            foreach (var rq in riders)
            {
                rq.Ticket = await _ridersTicketsService.GetRiderTicketAsync(rq.Id) ?? "";
                rq.Name = await _ridersTicketsService.GetRiderTicketAsync(rq.Id) ?? "";
            }
            var ridersSortedList = riders.OrderBy(x => x.Status).ToList();

            response.Data.RidersQResponse = ridersSortedList;
            response.Data.IsTripActive = true;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// Request for riders to Get their active reservations
        /// </summary>
        /// <returns></returns>
        // turn
        [HttpGet("Active")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Rider")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(SubFilter), Order = 3)]
        public async Task<IActionResult> GetActiveReservationForRiders()
        {
            GeneralResponse<ActiveReservationResponse> response = new GeneralResponse<ActiveReservationResponse>()
            {
                Message = _localizer["GeneralErrorMsg"].Value,
                Data = null
            };

            //var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            //clone.DateTimeFormat = CultureInfo.GetCultureInfo("en").DateTimeFormat;
            //Thread.CurrentThread.CurrentCulture = clone;
            //Thread.CurrentThread.CurrentUICulture = clone;
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            // Need to be changed to get also the accepted, confirmed, notified, ///
            var riderInQ = await _ridersQueueService.GetRiderActiveReservationAsync(userId);

            if (riderInQ == null)
            {
                response.Success = true;
                response.Message = _localizer["NoReservations"].Value; /*"لا يتوفر حجز حالي";*/
                response.Code = "";
                return StatusCode(404, response);
            }

            string ticket = "";
            string carCode = "";

            if (riderInQ.RidersQStatusLookupId == (int)RidersQStatusLookupEnum.Accepted
                || riderInQ.RidersQStatusLookupId == (int)RidersQStatusLookupEnum.Confirmed)
            {
                ticket = await _ridersTicketsService.GetRiderTicketAsync(riderInQ.Id);
                var carq = await _carsQueueService.GetQCurrentCarsQueueTurnAsync(riderInQ.PickupStationId);

                if (carq != null)
                {
                    carCode = carq.Car.CarCode;
                }
            }

            int turn = await _ridersQueueService.GetCountOfReservationBeforeMeAsync(riderInQ.ReservationDate, riderInQ.CreationDate,
                riderInQ.PickupStationId, riderInQ.Id);
            var psStation = await _pickupStationsService.GetPickupStationsByIdAsync(riderInQ.PickupStationId);
            var activeReservation = _mapper.Map<ActiveReservationResponse>(riderInQ);

            response.Data = activeReservation;
            response.Data.PickupStation = _mapper.Map<PickupStationsResponse>(psStation);
            response.Data.Turn = turn + 1;
            response.Data.IsSkiped = riderInQ.SkipCount > 0;
            response.Data.Ticket = string.IsNullOrWhiteSpace(ticket) ? "" : ticket;
            response.Data.CarCode = string.IsNullOrWhiteSpace(carCode) ? "" : carCode;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }


    }
}