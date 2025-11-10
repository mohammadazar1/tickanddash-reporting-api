using System;

namespace TickAndDashDAL.Models
{
    public class UserTransactions
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserTransactionTypeId { get; set; }
    }
}
