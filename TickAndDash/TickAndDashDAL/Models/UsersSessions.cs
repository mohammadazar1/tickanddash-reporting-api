using System;

namespace TickAndDashDAL.Models
{

    public class UsersSessions
    {
        public int Id { get; set; }

        public string UserAgent { get; set; }

        public string DeviceModel { get; set; }

        public string OSVersion { get; set; }

        public string Operation { get; set; }

        public string IP { get; set; }

        public int UserId { get; set; }

        public DateTime LogingDate { get; set; }

        public DateTime LogoutDate { get; set; }

    }

  
}
