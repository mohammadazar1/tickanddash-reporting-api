using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class DriverSkipCountRequest
    {
        /// <summary>
        /// SkipCount can skip from 1  to 3
        /// </summary>
        /// <example>3</example>
        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int SkipCount { get; set; }
    }


    public class GetDriverStatusResponse
    {
        /// <summary>
        /// Status
        /// </summary>
        /// <example>Active</example>
        public string Status { get; set; }



    }
}
