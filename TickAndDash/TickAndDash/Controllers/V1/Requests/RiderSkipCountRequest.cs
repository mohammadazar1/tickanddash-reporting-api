using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class RiderSkipCountRequest
    {
        /// <summary>
        /// SkipTime
        /// </summary>
        /// <example>00:15:00</example>
        [Required]
        public string SkipTime { get; set; }

    }



}
