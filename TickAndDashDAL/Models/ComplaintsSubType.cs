using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class ComplaintsSubType
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        public int ComplaintTypesId { get; set; }

    }

}
