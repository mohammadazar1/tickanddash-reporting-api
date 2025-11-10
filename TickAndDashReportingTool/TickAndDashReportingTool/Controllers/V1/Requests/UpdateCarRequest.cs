using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class UpdateCarRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int ItineraryId { get; set; }
        public string RegistrationPlate { get; set; }
        public string Model { get; set; }
        public int SeatCount { get; set; }
        public int LoggedInDriverId { get; set; }
    }
}
