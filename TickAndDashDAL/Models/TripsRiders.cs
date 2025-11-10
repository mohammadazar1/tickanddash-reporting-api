namespace TickAndDashDAL.Models
{
    public class TripsRiders
    {
        public int Id { get; set; }
        
        public int TripId { get; set; }
        public CarsTrips CarsTrip { get; set; }
        
        public int RiderId { get; set; }
        public Riders Rider { get; set; }
        public User User { get; set; }

        public int RiderQId { get; set; }
        public RidersQueue RidersQueue { get; set; }

    }
}
