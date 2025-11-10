using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class TotalFinancialBalance
    {
        public int UserId { get; set; }
        public string LicenseNumber { get; set; }
        public string Name { get; set; }
        public double DriverTransferedMoney { get; set; } = 0.0;
        public double DriverTripMoney { get; set; } = 0.0;
        public double PaymentsToDriver { get; set; } = 0.0; 
    }
}

