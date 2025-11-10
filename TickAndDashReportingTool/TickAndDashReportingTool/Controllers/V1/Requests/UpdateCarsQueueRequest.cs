using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDashReportingTool.Controllers.V1.Requests
{
    public class UpdateCarsQueueRequest
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int CarId { get; set; }
        public int SkipCount { get; set; }
        public int PickupStationId { get; set; }
        public int CarsQStatusLookupId { get; set; }
        public bool IsNotifiedAboutTurn { get; set; }
        public int Turn { get; set; }

    }
}
