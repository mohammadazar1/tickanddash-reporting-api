using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class BlacklistedUsersTokens
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool IsBlocked { get; set; }
        //public DateTime IsBlocked { get; set; }
    }
}
