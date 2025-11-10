using Dapper.Contrib.Extensions;

namespace TickAndDashDAL.Models
{
    public class PointOfSales
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int SiteId { get; set; }
        public Site Site { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string DCToken { get; set; }

        [Computed]
        public decimal Balance { get; set; }
    }


}
