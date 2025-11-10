using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class CreatePaymentToDriversRequest
    {
        public int DriverId { get; set; }
        public double PaymentAmount { get; set; }

    }
}
