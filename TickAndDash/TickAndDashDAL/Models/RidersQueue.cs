using System;

namespace TickAndDashDAL.Models
{
    public class RidersQueue
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ReservationDate { get; set; }
        public int RiderId { get; set; }
        public User User { get; set; }
        public int PickupStationId { get; set; }
        public PickupStations PickupStations { get; set; }
        public int RidersQStatusLookupId { get; set; }
        public RidersQStatusLookup RidersQStatusLookup { get; set; }
        public int SkipCount { get; set; }
        public int CountOfSeats { get; set; }
        public bool IsInQueue { get; set; }
        public int Turn { get; set; }
        public bool IsPresent { get; set; }

    }
}
