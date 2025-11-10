using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class Transportation_Itineraries
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }  
        
   

        public int FromSiteId { get; set; }
        public Site FromSite { get; set; }

        public int TowardSiteId { get; set; }
        public Site TowardSite { get; set; }
        
        public string Description { get; set; }
        public int ItineraryTypeLookupId { get; set; }
        public ItineraryTypeLookup ItineraryTypeLookup { get; set; }
        
        public bool IsActive { get; set; }

        public double Price { get; set; }

    }


}
