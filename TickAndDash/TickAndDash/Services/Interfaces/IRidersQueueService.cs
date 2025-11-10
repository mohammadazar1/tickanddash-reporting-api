using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TickAndDashDAL.Enums;
using TickAndDashDAL.Models;

namespace TickAndDash.Services.Interfaces
{
    public interface IRidersQueueService
    {

        Task<RidersQueue> GetRiderActiveReservationAsync(int riderId);
        Task<RidersQueue> GetActiveRidersInQWithStatusAsync(int riderId, RidersQStatusLookupEnum ridersQStatus);
        Task<RidersQueue> GetRidersInQWithStatusAsync(int riderQId, RidersQStatusLookupEnum status);

        Task<int> GetCountOfRidersAfterMyTurnAsync(int riderId, int psId);
        Task<int> GetCountOfReservationBeforeMeAsync(DateTime reservationTime, DateTime creationDate,  int psId, int riderQId);
        Task<int> GetRaiderDailyCancellationCountAsync(int riderId);
        Task<int> GetRaiderWeeklyCancellationCountAsync(int riderId);
        Task<int> GetRaiderCancellationCountAsync(int riderId);

        int GetQLastRiderTurn(int pickupId);
        int GetRiderTurn(int pickupId, int riderId);
        Task<List<RidersQueue>> GetAllAcitveRidersInPickupStationQWithStatus(RidersQStatusLookupEnum status, int ps);
        Task<List<RidersQueue>> GetActiveSeatViewRidersReservationInPickupStationAsync(int ps, string language);
        Task<List<RidersQueue>> GetFirstNRidersSeatsCountInPickupStationQAsync(int ps, int seatsCount);
        Task<List<RidersQueue>> GetRidersQOfCarTripAsync(int carQId);

        Task<int> AddToRidersQueueAsync(int riderId, int pickupId, RidersQStatusLookupEnum ridersQStatusLookupEnum, DateTime creationDate, DateTime reservationDate, int CountOfSeats);
        Task<bool> UpdateRiderStatusInQueueAsync(int riderQId, RidersQStatusLookupEnum status);
        Task<bool> UpdateRidersInQStatusAsync(List<int> riders, bool isInQueue);
        Task<bool> UpdateRiderPresenceStatusAsync(int riderQId);
        Task<bool> GetRiderPresenceStatusAsync(int riderQId);
        Task<bool> UpdateRiderSkipTimeAsync(int riderQId, TimeSpan timeSpan, DateTime ReservationDate);
        bool IsRiderInQueueWithStatus(int riderId, int psId, RidersQStatusLookupEnum status);
        Task<bool> IsRiderInAnyQueueAsync(int riderId);
        Task<RidersQueue> GetRiderInAnyQueueWithStatusAsync(int riderId, RidersQStatusLookupEnum status);

        Task<bool> CancelRiderReservationIfAnyAsync(int riderId);
        Task<int> CreateManualTicket(RidersQueue ridersQueue);
    }
}
