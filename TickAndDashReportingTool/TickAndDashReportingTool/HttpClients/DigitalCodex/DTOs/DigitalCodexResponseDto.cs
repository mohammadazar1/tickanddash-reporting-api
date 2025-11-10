namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class DigitalCodexResponseDto<T>
    {
        public bool Success { get; set; }
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public string Code { get; set; }
        public T Data { get; set; }
    }

    public class DigitalCodexGetBalanceResponse
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ReservedBalance { get; set; }
        public decimal BalanceCanBeUsed { get; set; }
    }

}