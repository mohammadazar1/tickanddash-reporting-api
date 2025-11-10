using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TickAndDashDAL.Models
{
    [Table("Callbacks")]
    public class Callback
    {
        public string MSISDN { get; set; }
        public string BillingStatus { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime NextActionDate { get; set; }
    }
}
