using System.Collections.Generic;

namespace TickAndDash.Controllers.V1.Responses
{
    public class PickupStationsResponse
    {
        public int PikcupId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Descriptions { get; set; }
        public string StationType { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int ItineraryId { get; set; }
        public string ItineraryName { get; set; }
        public decimal Radius { get; set; }
    }

    public class PushDriversToCarQueueResponse
    {
        public List<PickupStationsResponse> PickupStationsResponses { get; set; } = new List<PickupStationsResponse>();
        public int DriverRanking { get; set; }

    }
}
