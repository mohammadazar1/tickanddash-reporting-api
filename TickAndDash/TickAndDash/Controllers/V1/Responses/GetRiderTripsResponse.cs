using System;
using System.Collections.Generic;

namespace TickAndDash.Controllers.V1.Responses
{
    //public class GetRiderTripsResponse
    //{
    //    public int Id { get; set; }
    //    public string DriverName { get; set; }
    //    public DateTime CreationDate { get; set; }
    //    public string Itinerary { get; set; }
    //    public string CarRegistrationPlate { get; set; }
    //    public string CarModel { get; set; }
    //}

    public class GetUserBalanceResponse
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ReservedBalance { get; set; }
        public List<UserTransactionsData> UserTransactions { get; set; } = new List<UserTransactionsData>();
    }

    public class UserTransactionsData
    {
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
