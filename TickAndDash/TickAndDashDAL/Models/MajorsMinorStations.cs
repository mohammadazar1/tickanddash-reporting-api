namespace TickAndDashDAL.Models
{
    public class MajorsMinorStations
    {
        public int Id { get; set; }
        public int MainPickupStationId { get; set; }
        public PickupStations MainPickupStations { get; set; }
        public int MinorPickupStationId { get; set; }
        public PickupStations MinorPickupStations { get; set; }
        public int FromSiteId { get; set; }
        public int TowardSiteId { get; set; }
        public int TransItineraryId { get; set; }
        public int DurationInMinutes { get; set; }
        //public int SiteId { get; set; }
        //public Site Sites { get; set; }

    }

}
