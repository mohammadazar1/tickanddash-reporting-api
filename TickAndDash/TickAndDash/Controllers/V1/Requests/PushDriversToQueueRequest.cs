using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class PushDriversToQueueRequest
    {
        /// <summary>
        /// PickupStationId
        /// </summary>
        /// <example>4</example>
        [Range(1, 9999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int PickupStationId { get; set; }
    }
}
