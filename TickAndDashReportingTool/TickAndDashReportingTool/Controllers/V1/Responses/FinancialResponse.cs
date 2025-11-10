using System;

namespace TickAndDashReportingTool.Controllers.V1
{
    public class FinancialResponse
    {
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string RegisterPlate { get; set; }
        public string MobileNumber { get; set; }
        public string LicenseNumber { get; set; }
    }

    //public class POSTransactions
    //{
    //    public int Id { get; set; }
    //    public int POSId { get; set; }
    //    public string POSName { get; set; }
    //    public string MSISDN { get; set; }
    //    public decimal Amount { get; set; }
    //}

}