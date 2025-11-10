using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class GetAllByfilterPaymentsToDriversRequest
    {
        public DateTime From { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime To { get; set; } = DateTime.Now;
        public int DriverId { get; set; } = 0;
    }
}
