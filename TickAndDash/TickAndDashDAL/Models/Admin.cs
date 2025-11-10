using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class Admin
    {
        [MaxLength(40)]
        public string MSISDN { get; set; }
        public int UserId { get; set; }
        [MaxLength(30)]
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
    }
}
