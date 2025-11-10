using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class ConfirmRidersRequest
    {
        public int RiderQId { get; set; }
    }


    public class ConfirmRiderPresenceRequest
    {
        public string CarCode { get; set; }
    }

    public class CreateManualTicketRequest
    {
        //public int PickupStationId { get; set; }
        [Range(1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int countOfSeat { get; set; }
    }
}
