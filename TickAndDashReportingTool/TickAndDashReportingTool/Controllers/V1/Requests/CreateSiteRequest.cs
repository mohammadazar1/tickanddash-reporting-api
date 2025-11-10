using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class CreateSiteRequest
    {
        public string Name { get; set; }
        public int SitesTypeLookupId { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
