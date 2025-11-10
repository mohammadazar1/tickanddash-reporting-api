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
using TickAndDash.Filters;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using static TickAndDash.Filters.AuthorizationFilter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TickAndDash.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class DriversQueueController : ControllerBase
    {

        private readonly IPickupStationsService _pickupStationsService;
        private readonly IMapper _mapper;
        private readonly ICarsQueueService _carsQueueService;
        private readonly IRidersQueueService _ridersQueueService;
        private readonly IStringLocalizer<DriversQueueController> _localizer;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly ITransItinerariesService _transItinerariesService;
        private readonly INotificationService _notificationService;
        private readonly IUsersService _usersService;
        private readonly ITripsService _tripsService;
        private readonly IRidersTicketsServices _ridersTicketsServices;
        public DriversQueueController(IPickupStationsService pickupStationsService, IMapper mapper, ICarsQueueService carsQueueService,
            IRidersQueueService ridersQueueService, IStringLocalizer<DriversQueueController> localizer,
            ISystemConfigurationService systemConfigurationService, ITransItinerariesService transItinerariesService,
            INotificationService notificationService, IUsersService usersService, ITripsService tripsService,
            IRidersTicketsServices ridersTicketsServices)
        {
            _pickupStationsService = pickupStationsService;
            _mapper = mapper;
            _carsQueueService = carsQueueService;
            _ridersQueueService = ridersQueueService;
            _localizer = localizer;
            _systemConfigurationService = systemConfigurationService;
            _transItinerariesService = transItinerariesService;
            _notificationService = notificationService;
            _usersService = usersService;
            _tripsService = tripsService;
            _ridersTicketsServices = ridersTicketsServices;
        }



        // When he can reserve again 
        // is there any canellation choice
        // when loged out from the app do we need to clean all his reservations
        // when drvie iternet cut and he has an active reservation
        // trace his internet connectivity in database or using radis
        // if he is offline pop him out from the queue
        /// <summary> Request for pushing drivers to their main pickup stations Queue
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// **Codes**  <br/>
        /// 1.Ok, // Success
        /// 2.car_1, // Car already in the queue <br/>
        /// 3.PickUp_1 // pickup not active for this driver
        /// </remarks>
        /// <response code = "201"> Success </response>
        /// <response code = "403"> UnAuthorized </response>
        /// <response code = "401"> UnAuthorized </response>
        /// <response code = "422"> Failed </response>
        [HttpPost("")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<List<PushDriversToCarQueueResponse>>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> PushDriversToCarQueue([FromBody] PushDriversToQueueRequest request)
        {
            GeneralResponse<PushDriversToCarQueueResponse> response = new GeneralResponse<PushDriversToCarQueueResponse>()
            {
                Data = null
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }


            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            int.TryParse(User.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);

            // change in bus flow 
            bool isPickupActive = await _pickupStationsService.IsPickupstationActiveAndVaildForTheDriverAsync(carId, request.PickupStationId);

            if (!isPickupActive)
            {
                response.Message = _localizer["InvalidReservation"].Value; /*"عذرًا، لايمكنك حجز دور في هذا الموقف";*/
                response.Code = ValidationCodes.PickUp_1.ToString();
                return StatusCode(422, response);
            }

            //bool isCarInQ = await _carsQueueService.IsCarTurnActiveInTheStationAsync(carId, request.PickupStationId);
            bool isCarInQ = await _carsQueueService.IsCarInQueueAsync(carId);

            if (isCarInQ)
            {
                response.Message = _localizer["ActiveReservationError"].Value; /* "عذرًا، لديك حجز فعال حاليًا";*/
                response.Code = QueusCode.car_1.ToString();
                return UnprocessableEntity(response);
            }

            string limitInMinutes = await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToAccessDriverQAgian);
            if (!int.TryParse(limitInMinutes, out int timeLimit))
            {
                timeLimit = 5;
            }

            bool isBookingTimeLimited = await _carsQueueService.IsCarTimeLimitationValidToEnterTheQ(carId, request.PickupStationId, timeLimit);

            if (!isBookingTimeLimited)
            {
                response.Message = _localizer["BookingTimeLimitedMsg", limitInMinutes].Value;
                response.Code = ValidationCodes.book_1.ToString();
                return StatusCode(422, response);
            }

            bool isAdded = await _carsQueueService.AddCarToTheQueueAsync(carId, request.PickupStationId, userId);

            if (!isAdded)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value;
                response.Code = UnExpectedErrors.sql_1.ToString();
                return UnprocessableEntity(response);
            }

            var pickupStation = await _pickupStationsService.GetPickupStationsByIdAsync(request.PickupStationId);
            var psStations = await _pickupStationsService.GetPickupStationsEndPointsPickupStationsAsync(pickupStation.SiteId, pickupStation.TransItineraryId);
            var psResponse = _mapper.Map<List<PickupStationsResponse>>(psStations);

            //int myTurn = _carsQueueService.GetCountOfCarsInQueue(request.PickupStationId);
            int turn = await _carsQueueService.GetCarQTurnAsync(request.PickupStationId, carId);

            response.Success = true;
            response.Data = new PushDriversToCarQueueResponse();
            response.Data.PickupStationsResponses = psResponse;
            response.Data.DriverRanking = turn;
            response.Message = _localizer["SuccessMsg"].Value; /* "تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return StatusCode(201, response);
        }

        /// <summary> Request for leting drivers to skip their turns, they can only skip three cars
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// **Codes**  <br/>
        /// car_2, // No active reservation fro the  <br/>
        /// Data_2 // sql query did not affect any row
        /// </remarks>
        /// <response code = "201"> Success </response>
        /// <response code = "403"> UnAuthorized </response>
        /// <response code = "401"> UnAuthorized </response>
        /// <response code = "422"> Failed </response>
        [HttpPost("Skip")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 201)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]

        public async Task<IActionResult> SkipDriversToCarQueue([FromBody] DriverSkipCountRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);

            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carsQueue == null)
            {
                response.Message = _localizer["NoReservationError"].Value; /* "عذرًا، ليس لديك حجز فعال حاليًا";*/
                response.Code = QueusCode.car_2.ToString();
                return UnprocessableEntity(response);
            }

            if (carsQueue.Turn == 1)
            {
                response.Message = _localizer["YourTurnValidation"].Value;
                response.Code = QueusCode.car_4.ToString();
                return UnprocessableEntity(response);
            }

            int countOfCars = await _carsQueueService.GetCountOfCarsAfterMyTurnAsync(carId, carsQueue.PickupStationId);
            //int lastTurn = _carsQueueService.GetQLastCarTurn(carsQueue.PickupStationId);

            if (countOfCars < request.SkipCount)
            {
                response.Message = _localizer["SkipError", request.SkipCount].Value; /*$"عذرًا، لايتواجد {request.SkipCount} حافلات لديها حجز فعالك خلفك"*/;
                response.Code = QueusCode.car_2.ToString();
                return UnprocessableEntity(response);
            }

            //var countOfCars = _carsQueueService.GetCountOfCarsAfterMyTurn(carId, carsQueue.PickupStationId);

            //if (countOfCars < request.SkipCount)
            //{
            //    response.Message = $"عذرًا، لايتواجد {request.SkipCount} حافلات لديها حجز فعالك خلفك";
            //    response.Code = QueusCode.car_2.ToString();
            //    return UnprocessableEntity(response);
            //}
            // there is a trigger upon this action

            bool isSkiped = await _carsQueueService.UpdateCarSkipCountAsync(carId, request.SkipCount);

            if (!isSkiped)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ اثناء تحديث الترتيب";*/
                response.Code = ValidationCodes.Data_2.ToString();
                return UnprocessableEntity(response);
            }

            //int turn = await _carsQueueService.GetCarQTurnAsync(carsQueue.PickupStationId, carsQueue.CarId);

            //bool isUpdated = _carsQueueService.UpdateCarsTurnAfterSkip(carsQueue.PickupStationId, 0, turn + 1, (turn + 1 + request.SkipCount));
            //if (!isUpdated)
            //{
            //    response.Message = "";
            //    response.Code = UnExpectedErrors.sql_1.ToString();
            //    return UnprocessableEntity(response);
            //}

            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();
            response.Success = true;

            return StatusCode(201, response); ;
        }

        /// <summary> Request for Cancelling drivers turn in his pickupstation Q.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **Codes**  <br/>
        /// Auth_1,// token is not valid <br/>
        /// Auth_2 // rider session is expired due to opening another one <br/>
        /// car_2, // No active reservation fro the <br/>
        /// Data_2 // sql query did not affect any row <br/>
        /// </remarks>
        /// <response code = "204"> Success </response>
        /// <response code = "403"> UnAuthorized </response>
        /// <response code = "401"> UnAuthorized </response>
        /// <response code = "422"> Failed </response>
        [HttpPut("Cancellation")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 204)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> CancelDriverTurn()
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null
            };

            int.TryParse(User.FindFirstValue("CarId"), out int carId);

            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carsQueue == null)
            {
                response.Message = _localizer["NoReservationError"].Value; /*"عذرًا، ليس لديك حجز فعال حاليًا";*/
                response.Code = QueusCode.car_2.ToString();
                return UnprocessableEntity(response);
            }

            if (carsQueue.Turn == 1)
            {
                response.Message = _localizer["YourTurnValidation"].Value;
                response.Code = QueusCode.car_4.ToString();
                return UnprocessableEntity(response);
            }

            //bool Updated = _carsQueueService.UpdateCarsSkipCount(carsQueue.Id, carsQueue.PickupStationId);
            //if (!Updated)
            //{
            //    response.Message = "حدث خطأ ما، يرجى المحاولة مجددًا";
            //    response.Code = UnExpectedErrors.sql_1.ToString();
            //    return StatusCode(422, response);
            //}
            bool isUpdated = await _carsQueueService.UpdateCarStatusInQueueAsync(carId, carsQueue.PickupStationId, CarsQStatusLookupEnum.Cancelled);

            if (!isUpdated)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ اثناء تحديث الترتيب";*/
                response.Code = ValidationCodes.Data_2.ToString();
                return UnprocessableEntity(response);
            }

            //isUpdated = _carsQueueService.UpdateCarsTurnAfterCancelation(carId, carsQueue.Turn + 1);
            //if (!isUpdated)
            //{
            //    response.Message = "";
            //    response.Code = UnExpectedErrors.sql_1.ToString();
            //    return UnprocessableEntity(response);
            //}


            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response); ;
        }


        /// <summary>
        /// Request for the head driver in the queue to sent push notification for riders in riders q
        /// Not Used After RiderQ App
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// FCM Category:
        /// 
        /// **Codes** <br/>
        /// 1. Ok // Success
        /// 2. car_2 // Car is not in the queue <br/>
        /// 3. car_3 // it is not your turn <br/>
        /// 4. FCM_1 // FCM Not received <br/>
        /// </remarks>
        [HttpPost("ConfirmRider")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 204)]
        [TypeFilter(typeof(AuthFilter), Order = 1)]
        public async Task<IActionResult> ConfirmRiders([FromBody] ConfirmRidersRequest request)
        {
            throw new NotImplementedException();

            GeneralResponse<RidersQResponse> response = new GeneralResponse<RidersQResponse>()
            {
                Data = null
            };

            int.TryParse(User.FindFirstValue("CarId"), out int carId);
            var carQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carQueue == null)
            {
                response.Message = _localizer["NoReservationError"].Value; /*"الحافلة ليس لها دور مسجل!";*/
                response.Code = QueusCode.car_2.ToString();
                return NotFound(response);
            }


            //var carTurnId = _carsQueueService.GetCarTurnInPPickup(carQueue.PickupStationId);
            //if (carTurnId.CarId != carId)
            //{
            //    response.Success = false;
            //    response.Message = "عذرًا، يوجد سائقين أمامك في الدور";
            //    response.Code = QueusCode.car_3.ToString();
            //    return StatusCode(422, response);
            //}
            // Check IF it is from the first 7 riders
            //var ridersQueue = _queuesService.GetRidersInQ(request.RiderQId, RidersQStatusLookupEnum.Waiting);

            var ridersQueue = await _ridersQueueService.GetRidersInQWithStatusAsync(request.RiderQId, RidersQStatusLookupEnum.Waiting);
            if (ridersQueue == null)
            {
                response.Message = _localizer["NoPassenger"].Value; /* "عذرًا، الراكب غير متوفر الآن";*/
                response.Code = Generalcodes.Ok.ToString();
                return StatusCode(404, response);
            }

            decimal price = await _transItinerariesService.GetTransItineraryPriceByPickupStation(ridersQueue.PickupStationId);
            string lang = ridersQueue.User.Language;
            string title = lang == "ar" ? NotificationTexts.DriversQNotification.ConfirmNotificationMsgTitle_ar :
                    NotificationTexts.DriversQNotification.ConfirmNotificationMsgTitle_en;

            string body = lang == "ar" ?
                string.Format(NotificationTexts.DriversQNotification.ConfirmNotificationMsgBody_ar, price * ridersQueue.CountOfSeats) :
                    string.Format(NotificationTexts.DriversQNotification.ConfirmNotificationMsgBody_en, price * ridersQueue.CountOfSeats);

            //var pushNotificationDto = new PushNotificationDto
            //{
            //    data = new Data()
            //    {
            //        title = _localizer["ConfirmNotificationTitle"].Value, /*"تأكيد الحجز",*/
            //        body = _localizer["ConfirmNotificationMsg", price * ridersQueue.CountOfSeats].Value, /*"يرجى تاكيد الحجز",*/
            //        click_action = "VIEW_NOTIFICATION",
            //        category = "ConfirmReservation"

            //    },
            //    notification = new Notification
            //    {
            //        title = _localizer["ConfirmNotificationTitle"].Value, /*"تأكيد الحجز",*/
            //        body = _localizer["ConfirmNotificationMsg", price * ridersQueue.CountOfSeats].Value, /*"يرجى تاكيد الحجز",*/
            //        click_action = "VIEW_NOTIFICATION",
            //        category = "ConfirmReservation"
            //    },
            //    to = ridersQueue.User.FCMToken
            //}; 

            //var pushNotificationDto = new PushNotificationDto
            //{
            //    data = new Data()
            //    {
            //        title = title,
            //        body = body,
            //        click_action = "VIEW_NOTIFICATION",
            //        category = "ConfirmReservation"

            //    },
            //    notification = new Notification
            //    {
            //        title = title,
            //        body = body,
            //        click_action = "VIEW_NOTIFICATION",
            //        category = "ConfirmReservation"
            //    },
            //    to = ridersQueue.User.FCMToken
            //};

            bool isFCMSent = await _notificationService.SendNotificationAsync(ridersQueue.User.FCMToken
              , body, title, "VIEW_NOTIFICATION", "ConfirmReservation",
              "", RolesEnum.Rider);


            //bool isFCMSent = await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

            // changed
            if (!isFCMSent)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                response.Code = Generalcodes.FCM_1.ToString();
                return StatusCode(422, response);
            }
            else
            {
                //bool isUpdated = _queuesService.UpdateRiderStatusInQueueByRiderqId(request.RiderQId, RidersQStatusLookupEnum.notified);
                bool isUpdated = await _ridersQueueService.UpdateRiderStatusInQueueAsync(request.RiderQId, RidersQStatusLookupEnum.notified);

                if (!isUpdated)
                {
                    response.Message = _localizer["GeneralErrorMsg"].Value; /*"عذرًا، حدث خطأ ما يرجى المحاولة لاحقًا";*/
                    response.Code = Generalcodes.Internal.ToString();
                    return StatusCode(422, response);
                }
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value; /*"تم معالجة الطلب بنجاح";*/
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// Go from PS
        /// </summary>
        /// <returns></returns>
        [HttpPost("GO")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(GeneralResponse<object>), 204)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        public async Task<IActionResult> Go()
        {
            // drow them from the turn 
            GeneralResponse<List<StationsBooking>> response = new GeneralResponse<List<StationsBooking>>()
            {
                Data = new List<StationsBooking>()
            };
            
            int.TryParse(User.FindFirstValue("CarId"), out int carId);
            var carQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);
            
            if (carQueue == null)
            {
                response.Message = _localizer["NoReservationError"].Value; /*"الحافلة ليس لها دور مسجل!";*/
                response.Code = QueusCode.car_2.ToString();
                return NotFound(response); 
            }

            if (carQueue.Turn != 1)
            {
                response.Message = "عليك انتظار قدوم دورك للاقلاع";
                response.Code = QueusCode.car_3.ToString();
                return StatusCode(422, response);
            }

            var acceptedRider = await _ridersQueueService.GetAllAcitveRidersInPickupStationQWithStatus(RidersQStatusLookupEnum.Accepted, carQueue.PickupStationId);
            if (acceptedRider != null && acceptedRider.Count > 0)
            {
                response.Message = _localizer["NotConfirmedReservationError"].Value; /*"الحافلة ليس لها دور مسجل!";*/
                response.Code = "";
                return NotFound(response);
            }

            string _language = Request.Headers["Content-Language"];
            var ridersQ = await _ridersQueueService.GetActiveSeatViewRidersReservationInPickupStationAsync(carQueue.PickupStationId, _language);
            //if (ridersQ == null || !ridersQ.Any())
            //{
            //    response.Message = _localizer["NoReservations"].Value; /*"عذرًا، لاتوجد مقاعد محجوزة حاليًا";*/
            //    response.Code = ValidationCodes.Data_1.ToString();
            //    return NotFound(response);
            //}

            if (ridersQ != null && ridersQ.Any())
            {
                response.Data = new List<StationsBooking>();

                foreach (var rider in ridersQ)
                {
                    var station = response.Data.Find(x => x.Name == rider.PickupStations.Sites.Name);

                    if (station != null)
                    {
                        station.count += rider.CountOfSeats;
                    }
                    else
                    {
                        station = new StationsBooking
                        {
                            Name = rider.PickupStations.Sites.Name,
                            count = rider.CountOfSeats
                        };
                        response.Data.Add(station);
                    }
                    //rider.PickupStations.Sites.Name;
                }
            }

            var ridersQueues = await _ridersQueueService.GetRidersQOfCarTripAsync(carQueue.Id);
            // ALL confirm his presnet 
            bool someRiderNotPresetn = ridersQueues.Where(x => x.IsPresent == false && x.RidersQStatusLookupId != (int)RidersQStatusLookupEnum.Ticket).Any();

            if (ridersQueues.Count > 0)
            {
                if (someRiderNotPresetn)
                {
                    var lastRider = ridersQueues.Last();
                    if (DateTime.Now < lastRider.ReservationDate)
                    {
                        response.Message = _localizer["WaitingLastRider",
                            lastRider.ReservationDate.ToString("hh:mm:ss tt")].Value; /*"الحافلة ليس لها دور مسجل!";*/

                        if (response.Message.Contains("PM", StringComparison.OrdinalIgnoreCase))
                        {
                            response.Message = response.Message.Replace("PM", "م");
                        }
                        if (response.Message.Contains("AM", StringComparison.OrdinalIgnoreCase))
                        {
                            response.Message = response.Message.Replace("PM", "ص");
                        }

                        response.Success = false;
                        response.Code = "";
                        return BadRequest(response);
                    }
                }

                List<int> ridersQIds = ridersQueues.Select(x => x.Id).ToList();
                bool isUpdateds = await _ridersQueueService.UpdateRidersInQStatusAsync(ridersQIds, false);

                foreach (var rider in ridersQueues)
                {

                    bool isMain = await _pickupStationsService.IsMajorPickupStationAsync(rider.PickupStationId);

                    MajorsMinorStations majorsMinor = null;
                    if (!isMain)
                    {
                        majorsMinor = await _pickupStationsService.GetMajorsMinorStationsByMinorStationId(rider.PickupStationId);
                    }

                    string lang = rider.User.Language;
                    string title = lang == "ar" ? NotificationTexts.DriversQNotification.HappyMsgNotificationTitle_ar :
                             NotificationTexts.DriversQNotification.HappyMsgNotificationTitle_en;
                    string body = "";

                    if (lang == "ar")
                    {
                        body = isMain == true ?
                                              string.Format(NotificationTexts.DriversQNotification.HappyMsgNotificationBody_ar) :
                                                string.Format(NotificationTexts.DriversQNotification.HappyMsgNotificationForMinorStationBody_ar,
                                                majorsMinor?.DurationInMinutes);
                    }
                    else
                    {
                        body = isMain == true ?
                                             string.Format(NotificationTexts.DriversQNotification.HappyMsgNotificationBody_en) :
                                               string.Format(NotificationTexts.DriversQNotification.HappyMsgNotificationForMinorStationBody_en,
                                               majorsMinor?.DurationInMinutes);
                    }

                    //var pushNotificationDto = new PushNotificationDto
                    //{
                    //    data = new Data()
                    //    {
                    //        body = isMain == true ? _localizer["HappyMsgNotification"].Value : _localizer["HappyMsgNotificationForMinorStation", majorsMinor?.DurationInMinutes].Value,
                    //        title = _localizer["HappyMsgNotificationTitle"].Value,
                    //        category = "TheEnd"
                    //    },
                    //    notification = new Notification
                    //    {
                    //        body = isMain == true ? _localizer["HappyMsgNotification"].Value : _localizer["HappyMsgNotificationForMinorStation", majorsMinor?.DurationInMinutes].Value,
                    //        title = _localizer["HappyMsgNotificationTitle"].Value,
                    //        category = "TheEnd"
                    //    },
                    //    to = rider.User?.FCMToken
                    //};
                    //var pushNotificationDto = new PushNotificationDto
                    //{
                    //    data = new Data()
                    //    {
                    //        title = title,
                    //        body = body,
                    //        category = "TheEnd"
                    //    },
                    //    notification = new Notification
                    //    {
                    //        title = title,
                    //        body = body,
                    //        category = "TheEnd"
                    //    },
                    //    to = rider.User?.FCMToken
                    //};

                    await _notificationService.SendNotificationAsync(rider.User?.FCMToken
                        , body, title, "", "TheEnd",
                        "", RolesEnum.Rider);

                    //await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);
                }
                // warning
            }

            bool isUpdated = await _carsQueueService.UpdateCarStatusInQueueAsync(carId, carQueue.PickupStationId, CarsQStatusLookupEnum.Passed);

            if (!isUpdated)
            {
                response.Message = _localizer["GeneralErrorMsg"].Value; /*"حدث خطأ ما، يرجى المحاولة مجددًا";*/
                response.Code = UnExpectedErrors.sql_1.ToString();
                return StatusCode(422, response);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// Request for drivers to get Pickup station Current Queue live
        /// </summary>
        /// <returns></returns>
        [HttpGet("{pickupId:int}")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(TokenAuthorizationFilter), Order = 2)]
        public async Task<IActionResult> GetPickupQCars(int pickupId)
        {
            GeneralResponse<List<GetPickupQCarsResponse>> response = new GeneralResponse<List<GetPickupQCarsResponse>>()
            {
                Data = new List<GetPickupQCarsResponse>()
            };

            string role = User.FindFirstValue(ClaimTypes.Role);
            RolesEnum roleEnum = (RolesEnum)Enum.Parse(typeof(RolesEnum), role, true);

            if (roleEnum == RolesEnum.Driver)
            {

                int.TryParse(User.FindFirstValue("CarId"), out int carId);

                bool isPickupActive = await _pickupStationsService.IsPickupstationActiveAndVaildForTheDriverAsync(carId, pickupId);

                if (!isPickupActive)
                {
                    response.Message = _localizer["WrongPsStationMsg"]; /*"";*/
                    response.Code = ValidationCodes.PickUp_1.ToString();
                    return StatusCode(422, response);
                }


                var driver = await _carsQueueService.GetDriversInPickupQAsync(pickupId, CarsQStatusLookupEnum.InQueue);

                var drivesQ = _mapper.Map<List<GetPickupQCarsResponse>>(driver);

                //int count = 0;
                //foreach (var d in drivesQ)
                //{
                //    d.QTurn = ++count;
                //}

                response.Data = drivesQ;
                response.Success = true;
                response.Message = _localizer["SuccessMsg"].Value;
                response.Code = Generalcodes.Ok.ToString();
                return Ok(response);
            }
            else if (roleEnum == RolesEnum.Rider)
            {

            }

            return null;
        }

        /// <summary> 
        /// Request for the driver to get if he has an active reservation
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// **Codes**  <br/>
        /// </remarks>
        /// <response code = "201"> Success </response>
        /// <response code = "401"> UnAuthorized </response>
        /// <response code = "422"> Failed </response>
        [HttpGet("Active")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GetDriverActiveReservation()
        {
            GeneralResponse<GetDriverActiveReservationResponse> response = new GeneralResponse<GetDriverActiveReservationResponse>()
            {
                Data = null
            };

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);

            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);

            if (carsQueue == null)
            {
                response.Message = _localizer["NoReservationError"].Value;
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var pickupStation = await _pickupStationsService.GetPickupStationsByIdAsync(carsQueue.PickupStationId);
            var fromPickupResponse = _mapper.Map<PickupStationsResponse>(pickupStation);
            var psStations = await _pickupStationsService.GetPickupStationsEndPointsPickupStationsAsync(pickupStation.SiteId, pickupStation.TransItineraryId);
            var ToPickupsResponse = _mapper.Map<List<PickupStationsResponse>>(psStations);

            int turn = await _carsQueueService.GetCarQTurnAsync(carsQueue.PickupStationId, carId);
            response.Data = new GetDriverActiveReservationResponse()
            {
                Turn = turn
            };

            response.Data.From = fromPickupResponse;
            response.Data.To = ToPickupsResponse;
            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

        /// <summary>
        /// Used by driver who has the trun (turn = 1) to generate manual tickets for riders
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Generate/Ticket")]
        [Authorize(Roles = "Driver")]
        [MapToApiVersion("1.0")]
        [TypeFilter(typeof(LoggingFilter), Order = 1)]
        [TypeFilter(typeof(AuthFilter), Order = 2)]
        public async Task<IActionResult> GenerateManualTicket(CreateManualTicketRequest request)
        {
            GeneralResponse<object> response = new GeneralResponse<object>()
            {
                Data = null
            };

            if (!ModelState.IsValid)
            {
                var valResult = Helpers.ValidateModelState(ModelState, out string message);
                return StatusCode(valResult.Item1, valResult.Item2);
            }

            int.TryParse(User.FindFirstValue(ClaimsEnum.CarId.ToString()), out int carId);
            var carsQueue = await _carsQueueService.GetActiveCarInCarQAsync(carId);
            if (carsQueue == null || carsQueue.Turn != 1)
            {
                response.Message = _localizer["NotYourTurn"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return NotFound(response);
            }

            var seatViewRiders = await _ridersQueueService.GetActiveSeatViewRidersReservationInPickupStationAsync(carsQueue.PickupStationId, "ar");
            int seatViewCountOfSeats = 0;

            foreach (var seatView in seatViewRiders)
            {
                seatViewCountOfSeats += seatView.CountOfSeats;
            }

            if (seatViewCountOfSeats + request.countOfSeat > carsQueue.Car.SeatCount)
            {
                response.Message = _localizer["NoSeatsAvailable"].Value; /*"عذرًا، عليك حجز دور بدايًة";*/
                response.Code = ValidationCodes.Data_1.ToString();
                return BadRequest(response);
            }

            User user = await _usersService.GetManualTicketUserByNameAsync("ManualTicket");
            if (user == null)
            {
                int riderId = await _usersService.CreateManualTicketUserAsync();
                if (riderId > 0)
                    return BadRequest();
                user = new User { Id = riderId };
            }

            //..

            //ReservationDate = DateTime.Now.AddMinutes(int.Parse(reserveTimeLimit)),
            int riderQId = await _ridersQueueService.CreateManualTicket(new RidersQueue
            {
                PickupStationId = carsQueue.PickupStationId,
                RiderId = user.Id,
                CountOfSeats = request.countOfSeat,
                IsInQueue = true,
                ReservationDate = DateTime.Now.AddMinutes(-1),
                RidersQStatusLookupId = (int)RidersQStatusLookupEnum.Ticket
            });

            var driver = await _carsQueueService.GetActiveDriverInPickupQAsync(carsQueue.PickupStationId);
            RidersTickets ridersTickets = new RidersTickets()
            {
                RiderQId = riderQId,
                Ticket = $"{driver?.Car?.CarCode}_{user.Id}"
            };

            int Tid = await _ridersTicketsServices.AddRidersTickAsync(ridersTickets);
            ridersTickets.Ticket += $"_{Tid}";

            int tripId = await _tripsService.GetCarTripByCarsQueueIDAsync(carsQueue.Id);
            if (!await _tripsService.IsRiderQTripExistAsync(riderQId))
            {
                await _tripsService.AddToTripRiderAsync(riderQId, user.Id, tripId);
            }

            response.Success = true;
            response.Message = _localizer["SuccessMsg"].Value;
            response.Code = Generalcodes.Ok.ToString();

            return Ok(response);
        }

    }
}
