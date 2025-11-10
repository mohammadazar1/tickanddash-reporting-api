using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class UpdateDriverRequest
    {
        public string LicenseNumber { get; set; }
        public string Password { get; set; }
        public int CarId { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }

    }
}
