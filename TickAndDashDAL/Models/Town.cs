using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class Town
    {
        public int Id { get; set; }

        [MaxLength(10)]
        public string Name { get; set; }

        public decimal Lat { get; set; }

        public decimal Long { get; set; }

        public int CityId { get; set; }

    }


}
