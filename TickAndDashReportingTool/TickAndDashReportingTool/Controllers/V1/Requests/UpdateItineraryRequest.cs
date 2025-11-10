using System.ComponentModel.DataAnnotations;

namespace TickAndDashReportingTool.Controllers.V1
{
    public class UpdateItineraryRequest
    {
        [Required]
        public int Id { get; set; }

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