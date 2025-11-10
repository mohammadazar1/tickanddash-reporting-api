using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Services.Interfaces;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services
{
    public class RidersQueueService : IRidersQueueService
    {
        private readonly IRidersQueueDAL _ridersQueueDAL;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IPickupStationsService _pickupStationsService;

        public RidersQueueService(IRidersQueueDAL ridersQueueDAL, ISystemConfigurationService systemConfigurationService, IPickupStationsService pickupStationsService)
        {
            _ridersQueueDAL = ridersQueueDAL;
            _systemConfigurationService = systemConfigurationService;
            _pickupStationsService = pickupStationsService;
        }

        public async Task<int> AddToRidersQueueAsync(int riderId, int pickupId, RidersQStatusLookupEnum ridersQStatusLookupEnum,
            DateTime creationDate, DateTime reservationDate, int CountOfSeats)
        {
            RidersQueue ridersQueue = new RidersQueue()
            {
                RiderId = riderId,
                PickupStationId = pickupId,
                CreationDate = creationDate,
                RidersQStatusLookupId = (int)ridersQStatusLookupEnum,
                ReservationDate = reservationDate,
                CountOfSeats = CountOfSeats,
                IsInQueue = true
            };

            return await _ridersQueueDAL.AddToRidersQueueAsync(ridersQueue);
        }

        public int GetQLastRiderTurn(int pickupId)
        {
            return _ridersQueueDAL.GetQLastRiderTurn(pickupId);

        }

        public async Task<bool> IsRiderInAnyQueueAsync(int riderId)
        {
            return await _ridersQueueDAL.IsRiderInAnyQueueAsync(riderId);
        }

        public bool IsRiderInQueueWithStatus(int riderId, int psId, RidersQStatusLookupEnum status)
        {
            return _ridersQueueDAL.IsRiderInQueueWithStatus(riderId, psId, status);
        }

        public async Task<RidersQueue> GetRiderActiveReservationAsync(int riderId)
        {
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);


            return await _ridersQueueDAL.GetRiderActiveReservationAsync(riderId, notificationMinutesToExpire, timeLimitToCancellReservation);
        }

        public int GetRiderTurn(int pickupId, int riderId)
        {
            return _ridersQueueDAL.GetRiderTurn(pickupId, riderId);
        }

        public async Task<RidersQueue> GetRiderInAnyQueueWithStatusAsync(int riderId, RidersQStatusLookupEnum status)
        {
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            return await _ridersQueueDAL.GetRiderInAnyQueueWithStatusAsync(riderId, status, timeLimitToCancellReservation);
        }

        public async Task<bool> UpdateRiderStatusInQueueAsync(int riderQId, RidersQStatusLookupEnum status)
        {
            return await _ridersQueueDAL.UpdateRiderStatusInQueueAsync(riderQId, status);

        }

        public async Task<RidersQueue> GetActiveRidersInQWithStatusAsync(int riderId, RidersQStatusLookupEnum status)
        {
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpireReservation);

            return await _ridersQueueDAL.GetActiveRidersInQWithStatusAsync(riderId, status,
                timeLimitToCancellReservation, notificationMinutesToExpireReservation);
        }

        public async Task<int> GetCountOfRidersAfterMyTurnAsync(int riderId, int psId)
        {
            return await _ridersQueueDAL.GetCountOfRidersAfterMyTurnAsync(riderId, psId);
        }

        public async Task<List<RidersQueue>> GetAllAcitveRidersInPickupStationQWithStatus(RidersQStatusLookupEnum status, int ps)
        {
            //int notificationMinutesToExpire = 5;
            List<int> pickups = new List<int>();
            pickups.Add(ps);

            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            var minorPickups = await _pickupStationsService.GetMinorPickupStationsThatFollowsMainPickupStationAsync(ps, "ar");

            if (minorPickups != null || !minorPickups.Any())
            {
                var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                pickups.AddRange(minorPs);
            }

            //int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);
            return await _ridersQueueDAL.GetAllAcitveRidersInPickupStationQWithStatus(status, pickups,
                timeLimitToCancellReservation);
        }


        public async Task<int> GetCountOfReservationBeforeMeAsync(DateTime reservationTime, DateTime creationDate, int pickupId, int riderQId)
        {
            int psId = await _pickupStationsService.GetMinorPickupStationMainStaionId(pickupId);
            pickupId = psId == 0 ? pickupId : psId;
            List<int> pickups = new List<int>();
            pickups.Add(pickupId);

            var minorPickups = await _pickupStationsService.GetMinorPickupStationsThatFollowsMainPickupStationAsync(pickupId, "ar");

            if (minorPickups != null || !minorPickups.Any())
            {
                var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                pickups.AddRange(minorPs);
            }

            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            return await _ridersQueueDAL.GetCountOfReservationBeforeMeAsync(reservationTime, creationDate, pickups,
                notificationMinutesToExpire, timeLimitToCancellReservation, riderQId);
        }

        public async Task<bool> UpdateRiderSkipTimeAsync(int riderQId, TimeSpan timeSpan, DateTime ReservationDate)
        {
            return await _ridersQueueDAL.UpdateRiderSkipTimeAsync(riderQId, timeSpan, ReservationDate);
        }

        public async Task<List<RidersQueue>> GetFirstNRidersSeatsCountInPickupStationQAsync(int ps, int seatsCount)
        {
            //int seatCounts = 0;
            //List<RidersQueue> ridersQueues;
            //while (true)
            //{
            List<int> pickups = new List<int>();
            pickups.Add(ps);

            var minorPickups = await _pickupStationsService.GetMinorPickupStationsThatFollowsMainPickupStationAsync(ps, "ar");
            if (minorPickups != null || !minorPickups.Any())
            {
                var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                pickups.AddRange(minorPs);
            }

            int queueReservationtimeLimit = int.Parse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.ReserveTimeLimit));
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            return await _ridersQueueDAL.GetFirstNRidersInPickupStationQAsync(pickups, seatsCount, queueReservationtimeLimit, notificationMinutesToExpire, timeLimitToCancellReservation);
            //    foreach (var rider in ridersQueues)
            //    {
            //        if (seatCounts + rider.CountOfSeats < seatsCount)
            //        {
            //            ridersQueues.Add(rider);
            //        }
            //    }
            //}
            //return ridersQueues;
        }

        public async Task<List<RidersQueue>> GetRidersQOfCarTripAsync(int carQId)
        {
            return await _ridersQueueDAL.GetRidersQOfCarTripAsync(carQId);
        }

        public async Task<bool> UpdateRidersInQStatusAsync(List<int> ridersQIds, bool isInQueue)
        {
            return await _ridersQueueDAL.UpdateRidersInQStatusAsync(ridersQIds, isInQueue);

        }

        public async Task<int> GetRaiderDailyCancellationCountAsync(int riderId)
        {
            return await _ridersQueueDAL.GetRaiderDailyCancellationCountAsync(riderId);
        }

        public async Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId)
        {
            return await _ridersQueueDAL.GetRaiderWeeklyCancellationCountAsync(riderId);
        }

        public async Task<List<RidersQueue>> GetActiveSeatViewRidersReservationInPickupStationAsync(int ps, string language)
        {
            List<int> pickups = new List<int>();
            pickups.Add(ps);

            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);

            var minorPickups = await _pickupStationsService.GetMinorPickupStationsThatFollowsMainPickupStationAsync(ps, "ar");

            if (minorPickups != null || !minorPickups.Any())
            {
                var minorPs = minorPickups.Select(x => x.MinorPickupStations.Id).ToList();
                pickups.AddRange(minorPs);
            }

            return await _ridersQueueDAL.GetActiveSeatViewRidersReservationInPickupStationAsync(pickups,
                notificationMinutesToExpire, timeLimitToCancellReservation, language);
        }

        public async Task<RidersQueue> GetRidersInQWithStatusAsync(int riderQId, RidersQStatusLookupEnum status)
        {
            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.TimeLimitToCancellReservation), out int timeLimitToCancellReservation);


            int.TryParse(await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.NotificationMinutesToExpireReservation), out int notificationMinutesToExpire);


            return await _ridersQueueDAL.GetRidersInQWithStatusAsync(riderQId, status, timeLimitToCancellReservation);

        }

        public async Task<int> GetRaiderCancellationCountAsync(int riderId)
        {
            return await _ridersQueueDAL.GetRaiderCancellationCountAsync(riderId);
        }

        public async Task<bool> CancelRiderReservationIfAnyAsync(int riderId)
        {
            var ridersQueue = await _ridersQueueDAL.GetRidersInQAsync(riderId);

            if (ridersQueue == null)
            {
                return true;
            }

            return await _ridersQueueDAL.UpdateRiderStatusInQueueAsync(ridersQueue.Id, RidersQStatusLookupEnum.Canceled);
        }

        public async Task<bool> UpdateRiderPresenceStatusAsync(int riderQId)
        {
            return await _ridersQueueDAL.UpdateRiderPresenceStatusAsync(riderQId);
        }

        public async Task<bool> GetRiderPresenceStatusAsync(int riderQId)
        {
            return await _ridersQueueDAL.GetRiderPresenceStatusAsync(riderQId);

        }

        public async Task<int> CreateManualTicket(RidersQueue ridersQueue)
        {
            //string reserveTimeLimit = await _systemConfigurationService.GetSettingValueByKeyAsync(SettingKeyEnum.ReserveTimeLimit);
            //ridersQueue.ReservationDate = DateTime.Now.AddMinutes(int.Parse(reserveTimeLimit));
            
            return await _ridersQueueDAL.CreateManualTicket(ridersQueue);
        }
    }
}
