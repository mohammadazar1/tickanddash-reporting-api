using System;

namespace TickAndDash.Controllers.V1.Responses
{
    public class PushRidersToQueueResponse
    {
        //public int RidersInQ { get; set; }
        public int Turn { get; set; }
    }


    public class RiderSkipCountResponse
    {
        public DateTime ReservationDate { get; set; }
    }
}
