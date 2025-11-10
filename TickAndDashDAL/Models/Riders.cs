using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class Riders
    {

        public int UserId { get; set; }
        public User User { get; set; } = new User();
        [MaxLength(30)]
        public string MobileNumber { get; set; }
        public int LoginPincode { get; set; }
        public int GeneratedPincode { get; set; }
        public bool IsLoggedIn { get; set; }
        //public bool IsRegisterd { get; set; }
        public string UserAgent { get; set; }
        public string DeviceModel { get; set; }
        public string OSVersion { get; set; }
        public string Operation { get; set; }
        public string IP { get; set; }
        //public string DeviceName { get; set; }
        public char? Gender { get; set; }
        public string MobileOS { get; set; }
        public string Token { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime ActiveSubscriptionPeriod { get; set; }
        public DateTime UnsubDate { get; set; }
        public bool IsSubscribed { get; set; }
    }
}
