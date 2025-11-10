using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class PickupStationsLookup
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Type { get; set; }

        public bool IsActive { get; set; }
    }



}
