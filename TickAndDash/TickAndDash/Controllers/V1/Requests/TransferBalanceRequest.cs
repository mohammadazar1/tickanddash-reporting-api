using System;
using System.ComponentModel.DataAnnotations;

namespace TickAndDash.Controllers.V1.Requests
{
    public class TransferBalanceRequest
    {
        public string ToMobileNumber { get; set; }
        public decimal Amount { get; set; }
    }

    public class GetUserBalanceRequest
    {
        private static int year = DateTime.Now.Year;
        private static int month = DateTime.Now.Month;
        public string FromDate { get; set; } = $"{year}-{month}-1";
        public string ToDate { get; set; } = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.DaysInMonth(year, month)}";
    }

    public class RequestRefill
    {
        [Required(AllowEmptyStrings = false)]
        public string CarNumber { get; set; }

        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
    }

    public class RefillResponse
    {

        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Range(1, int.MaxValue)]
        public int RiderId { get; set; }

        public bool IsSuccess { get; set; }
    }
}
