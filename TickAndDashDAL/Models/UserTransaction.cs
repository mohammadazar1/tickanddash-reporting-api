using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class UserTransaction
    {
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string RegistrationPlate { get; set; }
        
        public string MobileNumber { get; set; }
        public string LicenseNumber { get; set; }
    }
}
