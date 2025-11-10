using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL.Interfaces
{
    public interface IRidersQueueDAL
    {
        List<RidersQueue> GetAllRidersInQ(RidersQStatusLookupEnum status, int ps);
        Task<List<RidersQueue>> GetFirstNRidersInPickupStationQAsync(List<int> ps, int seatsCount, int queueReservationtimeLimit, int notificationMinutesToExpire,
            int timeLimitToCancellReservation);
        Task<List<RidersQueue>> GetAllAcitveRidersInPickupStationQWithStatus
               (RidersQStatusLookupEnum status, List<int> ps, int timeLimitToCancellReservation);

        int GetQLastRiderTurn(int pickupId);
        int GetRiderTurn(int psId, int riderId);
        Task<RidersQueue> GetActiveRidersInQWithStatusAsync(int userId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation, int notificationMinutesToExpire);
        Task<int> GetCountOfRidersAfterMyTurnAsync(int riderId, int psId);
        Task<int> GetCountOfReservationBeforeMeAsync(DateTime reservationTime, DateTime creationDate, List<int> psId, int notificationMinutesToExpire,
            int timeLimitToCancellReservation, int riderQId);
        Task<int> CreateManualTicket(RidersQueue ridersQueue);
        Task<List<RidersQueue>> GetRidersQOfCarTripAsync(int carQId);
        Task<int> GetRaiderCancellationCountAsync(int riderId);
        Task<int> AddToRidersQueueAsync(RidersQueue ridersQueue);
        Task<bool> UpdateRiderStatusInQueueAsync(int riderQId, RidersQStatusLookupEnum status);

        //bool UpdateRiderSkipCount(int riderId, int skipCount);
        Task<bool> UpdateRiderSkipTimeAsync(int riderIQd, TimeSpan timeSpan, DateTime ReservationDate);
        Task<bool> UpdateRidersInQStatusAsync(List<int> riders, bool isInQueue);
        bool IsRiderInQueueWithStatus(int riderId, int psId, RidersQStatusLookupEnum status);
        Task<RidersQueue> GetRiderInAnyQueueWithStatusAsync(int riderId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation);
        Task<bool> IsRiderInAnyQueueAsync(int riderId);

        //bool DoesRiderHasAnActiveReservationInTheQueue(int riderId);
        List<RidersQueue> GetAllByPickupStationId(int id);
        //List<RidersQueue> GetAllByPickupStationId(int id);

        Task<List<RidersQueue>> GetActiveSeatViewRidersReservationInPickupStationAsync(List<int> pickups,
            int notificationMinutesToExpire, int timeLimitToCancellReservation, string language);
        Task<RidersQueue> GetRidersInQWithStatusAsync(int riderQId, RidersQStatusLookupEnum status, int timeLimitToCancellReservation);
        Task<RidersQueue> GetRidersInQAsync(int riderQId, RidersQStatusLookupEnum status);
        Task<RidersQueue> GetRidersInQAsync(int riderId);
        Task<RidersQueue> GetRiderActiveReservationAsync(int riderId, int notificationMinutesToExpire,
            int timeLimitToCancellReservation);
        int GetPickupStationsIdForRiderInQueueWithStatus(int riderId, RidersQStatusLookupEnum status);
        int GetCountOfRidersInFrontOfMyTurn(DateTime reservationdatetime, int psId);
        //bool CancelAllExpiredReservation(int riderId);
        bool CancelRiderReservation(int riderId);
        Task<int> GetRaiderDailyCancellationCountAsync(int riderId);
        Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId);
        bool UpdateRiderIsInQueueStatus(int riderId, bool isInQueue);
        bool UpdateRiderStatusInQueueByRiderqId(int riderQId, RidersQStatusLookupEnum status);
        Task UpdateRiderQNotifiedBookingWithNoUserResponseToAccepted(List<int> pickups, int notificationMinutesToExpire,
            int timeLimitToCancellReservation);
        Task<List<RidersQueue>> GetRiderQNotifiedBookingWithNoUserResponseAsync(List<int> pickups, int notificationMinutesToExpire,
          int timeLimitToCancellReservation);
        
            Task<bool> UpdateRiderPresenceStatusAsync(int riderQId);
        Task<bool> GetRiderPresenceStatusAsync(int riderQId);
    }
}
