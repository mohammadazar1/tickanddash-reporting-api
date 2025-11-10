using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class ConfirmRegistrationRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }

        [Range(1000, 9999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Pincode { get; set; }
    }
}
