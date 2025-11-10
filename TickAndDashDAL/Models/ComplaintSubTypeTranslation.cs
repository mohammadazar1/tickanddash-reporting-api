using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;

namespace TickAndDashDAL.Models
{
    [Table("ComplaintsSubTypeTranslations")]
    public class ComplaintSubTypeTranslation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SubType { get; set; }

        [Required]
        [MaxLength(10)]
        public string Lang { get; set; }

        [Required]
        public int ComplaintsSubTypeId { get; set; }

    }

}
