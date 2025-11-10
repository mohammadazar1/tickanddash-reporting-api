using System.ComponentModel.DataAnnotations;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class CreateDriverRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string LicenseNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [MinLength(9)]
        public string MSISDN { get; set; } = "";

        //[Required(AllowEmptyStrings = false)]
        public string DriverName { get; set; } = "";


        //[Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = "";

    }
}
