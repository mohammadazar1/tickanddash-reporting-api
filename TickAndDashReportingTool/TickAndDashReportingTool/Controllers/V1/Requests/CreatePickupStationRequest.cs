using System.ComponentModel.DataAnnotations;

namespace TickAndDashReportingTool.Controllers.V1
{
    public class CreatePickupStationRequest
    {
        [Required]
        public int TransItineraryId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public decimal Radius { get; set; }
        public string Description { get; set; }
        [Required]
        public int PickupTypeId { get; set; }
        [Required]
        public int SiteId { get; set; }

        public int DestinationMainPickup { get; set; }

        public int To { get; set; }

        public string NameAr { get; set; } = "";
        public string NameEn { get; set; } = "";


    }
}