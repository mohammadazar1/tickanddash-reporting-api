using TickAndDashDAL.Models;

namespace TickAndDashDAL.DAL
{
    public class Driver
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string LicenseNumber { get; set; }
        public string Password { get; set; } = "";
        public string MobileNumber { get; set; }
        public string MobileOS { get; set; }
        public string Address { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }


    }


    public class DriverWithName
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string LicenseNumber { get; set; }
        public string Password { get; set; } = "";
        public string MobileNumber { get; set; }
        public string MobileOS { get; set; }
        public string Address { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }
}
