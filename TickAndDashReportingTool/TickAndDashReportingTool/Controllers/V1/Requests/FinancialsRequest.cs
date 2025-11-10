using System;
using TickAndDashReportingTool.Enums;

namespace TickAndDashReportingTool.Controllers.V1
{
    public class FinancialsRequest
    {
        public int CarId { get; set; } = 0;
        public DateTime FromDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public int ItineraryId { get; set; } = 0;
        public string Frequancy { get; set; } = FrequanciesTypes.Daily.ToString();
    }
}