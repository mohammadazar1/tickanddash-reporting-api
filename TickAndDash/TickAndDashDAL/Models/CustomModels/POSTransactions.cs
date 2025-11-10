using System;

namespace TickAndDashDAL.Models.CustomModels
{

    public class POSTransactions
    {
        public int Id { get; set; }
        public int POSId { get; set; }
        public string POSName { get; set; }
        public string MSISDN { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
    }

}
