using System;

namespace TickAndDashReportingTool.Controllers.V1
{
    public class CreateTransferBalanceRequest
    {
        public string MSISDN { get; set; }

        public decimal Amount { get; set; }
    }

    public class GetTopUpTransactions
    {
        public DateTime From { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime To { get; set; } =
            new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        public string MobileNumber { get; set; }
        public int POSId { get; set; }
    }
}