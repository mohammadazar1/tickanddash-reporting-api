using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class Site
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(10)]
        public string Name { get; set; }
        public int SitesTypeLookupId { get; set; }
        public SiteLookup SiteLookup { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsActive { get; set; }

    }
}
