using System;

namespace TickAndDashDAL.Models
{
    public class CarsQueue
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int SkipCount { get; set; }
        public int Skip { get; set; }
        public int PickupStationId { get; set; }
        public PickupStations PickupStation { get; set; }
        public PickupStationsTranslations pickupStationsTranslations { get; set; }
        public int CarsQStatusLookupId { get; set; }
        public CarsQStatusLookup CarsQStatusLookup { get; set; }
        public bool IsNotifiedAboutTurn { get; set; }
        public int Turn { get; set; }
        public DateTime LeftingQTime { get; set; }
    }
}
