using System;
using System.Collections.Generic;
using System.Text;
using TickAndDashDAL.DAL;

namespace TickAndDashDAL.Models
{
    public class PaymentToDriver
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public double PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
