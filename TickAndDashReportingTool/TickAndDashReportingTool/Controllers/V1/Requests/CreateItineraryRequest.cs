using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class CreateItineraryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int FromSiteId { get; set; }
        [Required]
        public int TowardSiteId { get; set; }
        [Required]
        public int ItinerayTypeLookupId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public double Price { get; set; }

        public string Description { get; set; }
    }
}
