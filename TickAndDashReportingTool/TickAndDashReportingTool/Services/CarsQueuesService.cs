using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;
using TickAndDashReportingTool.Controllers.V1;
using TickAndDashReportingTool.Controllers.V1.Requests;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Services
{
    public class CarsQueuesService : ICarsQueuesService
    {
        private readonly ICarsQueueDAL _carsQueueDAL;
        private readonly ISystemConfigurationDAL _systemConfigurationDAL;
        private readonly IRidersQueueDAL _ridersQueueDAL;
        private readonly IRidersDAL _ridersDAL;
        private readonly IDigitalCodexClient _digitalCodex;
        private readonly ITransItinerariesDAL _transItinerariesDAL;
        private readonly ICarsTripsDAL _carsTripsDAL;
        private readonly IFCMHttpClient _fCMHttpClient;
        private readonly ITripsRidersDAL _tripsRidersDAL;
        private readonly IUserTransactionsDAL _userTransactionsDAL;
        private readonly IPickupStationsDAL _pickupStationsDAL;

        private readonly IMajorsMinorStationsDAL _minorPickupStationsDAL;

        public CarsQueuesService(ICarsQueueDAL carsQueueDAL, ISystemConfigurationDAL systemConfigurationDAL, IRidersQueueDAL ridersQueueDAL, IRidersDAL ridersDAL, IDigitalCodexClient digitalCodex, ITransItinerariesDAL transItinerariesDAL, ICarsTripsDAL carsTripsDAL, IFCMHttpClient fCMHttpClient, ITripsRidersDAL tripsRidersDAL, IUserTransactionsDAL userTransactionsDAL, IPickupStationsDAL pickupStationsDAL, IMajorsMinorStationsDAL majorsMinorStationsDAL)
        {
            _carsQueueDAL = carsQueueDAL;
            _systemConfigurationDAL = systemConfigurationDAL;
            _ridersQueueDAL = ridersQueueDAL;
            _ridersDAL = ridersDAL;
            _digitalCodex = digitalCodex;
            _transItinerariesDAL = transItinerariesDAL;
            _carsTripsDAL = carsTripsDAL;
            _fCMHttpClient = fCMHttpClient;
            _tripsRidersDAL = tripsRidersDAL;
            _userTransactionsDAL = userTransactionsDAL;
            _pickupStationsDAL = pickupStationsDAL;
            _minorPickupStationsDAL = majorsMinorStationsDAL;
        }

        public async Task<bool> ForceGo(ForceGoRequest forceGoRequest)
        {
            var carQueue = await _carsQueueDAL.GetActiveCarInCarQAsync(forceGoRequest.CarId);
            if (carQueue == null || carQueue.Turn != 1)
                return false;

            Driver driver = await _carsTripsDAL.GetDriverByCarQIdAsync(carQueue.Id);
            /* Get All Accepted Riders (still not confirmed) */
            //int.TryParse(await _systemConfigurationDAL.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation),
            //    out int timeLimitToCancellReservation);

            List<int> pickups = new List<int>();
            pickups.Add(forceGoRequest.PickupStationId);

            int.TryParse(await _systemConfigurationDAL.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationDAL.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);


            var minorPickups = await _minorPickupStationsDAL.GetMinorPickupStationsThatFollowsMainPickupStationAsync(forceGoRequest.PickupStationId, "ar");
            if (minorPickups != null || !minorPickups.Any())
            {
                var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                pickups.AddRange(minorPs);
            }

            var acceptedRider = await _ridersQueueDAL.GetAllAcitveRidersInPickupStationQWithStatus(RidersQStatusLookupEnum.Accepted, pickups,
                timeLimitToCancellReservation);

            /* Deduct balance form Accepted riders */
            decimal price = await _transItinerariesDAL.GetTransItineraryPriceByPickupStation(forceGoRequest.PickupStationId);

            foreach (var riderAccepted in acceptedRider)
            {
                var rider = await _ridersDAL.GetRiderByIdAsync(riderAccepted.RiderId);
                var transferResponse = await _digitalCodex.TransferBalanceAsync(new TransferBalanceRequest
                {
                    MobileNumber = driver.MobileNumber,
                    TransferBalance = price,
                    TransferCurrency = "ILS",
                    Username = "",
                    Token = rider.Token
                });

                if (transferResponse.Success)
                {
                    var carTripId = await _carsTripsDAL.GetCarTripByCarsQueueIDAsync(carQueue.Id);

                    if (carTripId > 0)
                    {
                        bool isAdded = await _tripsRidersDAL.AddToTripRiderAsync(riderAccepted.Id, rider.UserId, carTripId);

                        if (!isAdded)
                        {
                            // log warning
                        }
                    }

                    await _userTransactionsDAL.AddUserTransactionAsync(new UserTransactions
                    {
                        CreationDate = DateTime.Now,
                        FromUserId = rider.UserId,
                        ToUserId = driver.UserId,
                        Type = UserTransactionsTypesEnum.TripBilling.ToString(),
                        Amount = price * riderAccepted.CountOfSeats,
                        UserTransactionTypeId = (int)UserTransactionsTypesEnum.TripBilling
                    });
                }
                else
                {
                    return false;
                }
            }

            /* Update Riders Status */
            var ridersQueues = await _ridersQueueDAL.GetRidersQOfCarTripAsync(carQueue.Id);
            if (ridersQueues.Count > 0)
            {
                List<int> ridersQIds = ridersQueues.Where(x => x.RidersQStatusLookupId != (int)RidersQStatusLookupEnum.Ticket).Select(x => x.Id).ToList();
                ridersQIds.ForEach(async x =>
                {
                    bool isUpdatedRiderAcc = await _ridersQueueDAL.UpdateRiderStatusInQueueAsync(x, RidersQStatusLookupEnum.Confirmed);
                });

                bool isUpdateds = await _ridersQueueDAL.UpdateRidersInQStatusAsync(ridersQueues.Select(x => x.Id).ToList(), false);
            }

            if (ridersQueues.Count > 0)
            {
                foreach (var rider in ridersQueues)
                {
                    bool isMain = await _pickupStationsDAL.IsMajorPickupStationAsync(rider.PickupStationId);

                    MajorsMinorStations majorsMinor = null;
                    if (!isMain)
                    {
                        majorsMinor = await _minorPickupStationsDAL.GetMajorsMinorStationsByMinorStationId(rider.PickupStationId);
                    }

                    string lang = rider.User.Language;
                    string title = lang == "ar" ? "مع السلامة" : "";
                    string body = "";

                    if (lang == "ar")
                    {
                        body = isMain == true ?
                                            "نتمنى لك رحلة ممتعة وآمنة" :
                                                string.Format("سيصل السائق إلى المحطة خلال {0} دقائق ، يرجى التواجد هناك",
                                                majorsMinor?.DurationInMinutes);
                    }
                    else
                    {
                        body = isMain == true ?
                                             "We wish you a happy and safe journey" :
                                               string.Format("The Driver will be at the station within {0} minutes, please be there",
                                               majorsMinor?.DurationInMinutes);
                    }

                    var pushNotificationDto = new PushNotificationDto
                    {
                        data = new Data()
                        {
                            title = title,
                            body = body,
                            category = "TheEnd",
                            sound = "default"
                        },
                        notification = new Notification
                        {
                            title = title,
                            body = body,
                            category = "TheEnd",
                            sound = "default"
                        },
                        to = rider.User?.FCMToken
                    };

                    //var pushNotificationDto = new PushNotificationDto
                    //{
                    //    data = new Data()
                    //    {
                    //        body = rider.User.Language == "ar" ? "نتمنى لك رحلة سعيدة" : "We wish you a happy and safe journey",
                    //        title = rider.User.Language == "ar" ? "رحلة سعيدة" : "Goodbye",
                    //        category = "TheEnd"
                    //    },
                    //    notification = new Notification
                    //    {
                    //        body = rider.User.Language == "ar" ? "نتمنى لك رحلة سعيدة" : "We wish you a happy and safe journey",
                    //        title = rider.User.Language == "ar" ? "رحلة سعيدة" : "Goodbye",
                    //        category = "TheEnd"
                    //    },
                    //    to = rider.User?.FCMToken
                    //};
                    await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);
                }
                // warning
            }//

            /* Update Car Status */
            var isUpdated = await _carsQueueDAL.UpdateCarStatusInQueueAsync(carQueue.CarId, forceGoRequest.PickupStationId, CarsQStatusLookupEnum.Passed);

            if (!isUpdated)
                return false;
            else
            {


                PushNotificationDto pushNotificationDto = new PushNotificationDto
                {
                    data = new Data()
                    {
                        body = driver.User.Language == "ar" ? "تم الضغط على زر مغادرة بواسطة مدير الموقع" : "The Leave button was pressed by the site administrator",
                        title = driver.User.Language == "ar" ? "مغادرة" : "Goodbye",
                        category = "Go",
                        sound = "default"
                    },
                    to = driver.User?.FCMToken
                };

                if (driver.MobileOS?.ToLower() != "android")
                {
                    pushNotificationDto.notification = new Notification
                    {
                        body = driver.User.Language == "ar" ? "تم الضغط على زر مغادرة بواسطة مدير الموقع" : "The Leave button was pressed by the site administrator",
                        title = driver.User.Language == "ar" ? "مغادرة" : "Goodbye",
                        category = "Go",
                        sound = "default"
                    };
                }

                //var pushNotificationDto = new PushNotificationDto
                //{
                //    data = new Data()
                //    {
                //        body = driver.User.Language == "ar" ? "تم الضغط على زر مغادرة بواسطة مدير الموقع" : "The Leave button was pressed by the site administrator",
                //        title = driver.User.Language == "ar" ? "مغادرة" : "Goodbye",
                //        category = "Go"
                //    },
                //    notification = new Notification
                //    {
                //        body = driver.User.Language == "ar" ? "تم الضغط على زر مغادرة بواسطة مدير الموقع" : "The Leave button was pressed by the site administrator",
                //        title = driver.User.Language == "ar" ? "مغادرة" : "Goodbye",
                //        category = "Go"
                //    },
                //    to = driver.User?.FCMToken
                //};

                await _fCMHttpClient.PushNotificationsAsync(pushNotificationDto);

            }

            return true;
        }

        public IList<CarsQueue> GetAll(GetCarsQueueRequest getCarsQueueRequest)
        {
            return _carsQueueDAL.GetAllForTodayByItineraryId(getCarsQueueRequest.ItineraryId, lang: getCarsQueueRequest.Language);
        }

        public async Task<CarsQueue> GetQCurrentCarsQueueTurnAsync(int pickupId)
        {
            return await _carsQueueDAL.GetQCurrentCarsQueueTurnAsync(pickupId);
        }

        public bool Update(UpdateCarsQueueRequest updateCarsQueueRequest)
        {
            return _carsQueueDAL.Update(new CarsQueue
            {
                Id = updateCarsQueueRequest.Id,
                CarId = updateCarsQueueRequest.CarId,
                SkipCount = updateCarsQueueRequest.SkipCount,
                Turn = updateCarsQueueRequest.Turn,
                IsNotifiedAboutTurn = updateCarsQueueRequest.IsNotifiedAboutTurn,
                CarsQStatusLookupId = updateCarsQueueRequest.CarsQStatusLookupId
            });
        }
    }
}
