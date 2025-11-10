using System.Collections.Generic;

namespace TickAndDash.Controllers.V1.Responses
{
    public class RidersQResponse
    {
        public int Id { get; set; }
        public int CountOfSeats { get; set; }
        public string ReservationDate { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string PickupStation { get; set; }
        public string Ticket { get; set; }
        public bool IsPresent { get; set; }

    }

    public class SeatViewResponse
    {
        public List<RidersQResponse> RidersQResponse { get; set; } = new List<RidersQResponse>();
        public bool IsTripActive { get; set; } = false;

    }


    public class StationsBooking
    {
        public string Name { get; set; }
        public int count { get; set; }
    }

    public class GetAllPointOfSales
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string SiteName { get; set; }
    }
}
