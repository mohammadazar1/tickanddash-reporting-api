using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;

namespace TickAndDashDAL.Models
{
    [Table("ComplaintTypesTranslations")]
    public class ComplaintTypeTranslation
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int ComplaintTypesId { get; set; }

        [Required]
        [MaxLength(300)]
        public string Type { get; set; }

        [Required]
        [MaxLength(10)]
        public string Lang { get; set; }

    }

}
