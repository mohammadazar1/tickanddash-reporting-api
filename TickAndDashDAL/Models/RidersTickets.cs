using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class RidersTickets
    {
        public int Id { get; set; }
        public int RiderQId { get; set; }
        public RidersQueue RidersQueue { get; set; } 
        public string Ticket { get; set; }

    }
}
