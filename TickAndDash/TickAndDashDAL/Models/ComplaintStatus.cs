using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class ComplaintStatus
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; }

        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

    }

}
