using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class ResentPincodeRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }
    }
}
