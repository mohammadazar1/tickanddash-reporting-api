using System;

namespace TickAndDashDAL.Models
{
    public class BillingLogs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime BillingTimeStamp { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string Response { get; set; }
    }
}
