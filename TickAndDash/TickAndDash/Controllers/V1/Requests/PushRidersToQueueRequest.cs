using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class PushRidersToQueueRequest
    {
   
        [Range(1, 9999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int PickupStationId { get; set; }

        [Required]
        public string ReservationDate { get; set; }

       
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CountOfSeats { get; set; }
    }



}
