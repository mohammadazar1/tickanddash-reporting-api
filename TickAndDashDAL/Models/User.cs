using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDashDAL.Models
{
    public class User
    {
        public int Id { get; set; } = 0;

        [MaxLength(40)]
        public string Name { get; set; } = "";


        //[MaxLength(300)]
        //public string RefreshToken { get; set; }

        public DateTime CreationDate { get; set; }

        public int RoleId { get; set; } = 0;

        public bool IsActive { get; set; } = false;

        public string FCMToken { get; set; }

        public string Token { get; set; }

        public string Language { get; set; } = "en";

    }
}
