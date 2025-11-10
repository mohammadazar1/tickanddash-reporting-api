using System;
using System.Collections.Generic;
using System.Text;

namespace TickAndDashDAL.Models
{
    public class UserTransactionFilter
    {
        public int CarId { get; set; } = 0;
        public DateTime FromDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public int ItineraryId { get; set; } = 0;
        public string Frequancy { get; set; }
    }
}
