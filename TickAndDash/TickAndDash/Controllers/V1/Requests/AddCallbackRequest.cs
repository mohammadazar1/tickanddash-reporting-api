using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class AddCallbackRequest
    {

        public string ServiceId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MSISDN { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Status { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime TimeStamp { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime NextActionDate { get; set; }

    }
}
