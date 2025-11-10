using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class CreateCarRequest
    {
        public bool IsActive { get; set; }
        public int ItineraryId { get; set; }
        public string RegistrationPlate { get; set; }
        public string Model { get; set; }
        public string ModelYear { get; set; }
        public int SeatCount { get; set; }
        public int LoggedInDriverId { get; set; }
        [Required]
        public string CarCode { get; set; }
    }
}
