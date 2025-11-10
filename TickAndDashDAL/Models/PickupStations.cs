using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class PickupStations
    {
        public int Id { get; set; }
        public int TransItineraryId { get; set; }

        public Transportation_Itineraries Transportation_Itineraries { get; set; }

        public bool IsActive { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal Radius { get; set; }

        [MaxLength(300)]
        public string Descriptions { get; set; }

        [MaxLength(300)]
        public string Name { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }


        [Required]
        public int PickupTypeId { get; set; }

        public PickupStationsLookup PickupStationsLookup { get; set; }

        public int SiteId { get; set; }

        public Site Sites { get; set; }

        public string TransName { get; set; }

    }

}
