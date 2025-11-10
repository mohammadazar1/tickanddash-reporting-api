namespace TickAndDash.HttpClients.GeoClients.DTOs
{
    public class DigitalCodexResponseDto<T>
    {
        public string StatusCode { get; set; }
        public bool Success { get; set; }
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public string Code { get; set; }
        public T Data { get; set; }
    }

    public class DigitalCodexRegisterUserResponse
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool IsAutherized { get; set; }
    }

    public class DigitalCodexGetBalanceResponse
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ReservedBalance { get; set; }
        public decimal BalanceCanBeUsed { get; set; }
    }


    public class ReserveBalanceRequest
    {
        public decimal ReservationBalance { get; set; }
        public string ReservationCurrency { get; set; }
        public int ReservationPeriod { get; set; }
        public string Token { get; set; }
    }

    public class CancelReservationRequest
    {
        public string ReservationCurrency { get; set; }
    }


    public class TransferBalanceRequest
    {
        public decimal TransferBalance { get; set; }
        public string TransferCurrency { get; set; } = "ILS";
        public string MobileNumber { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }

    public class SubscribeRequest
    {
        public int ServiceId { get; set; }
        public string Token { get; set; }
        public string Language { get; set; }
    }

}
