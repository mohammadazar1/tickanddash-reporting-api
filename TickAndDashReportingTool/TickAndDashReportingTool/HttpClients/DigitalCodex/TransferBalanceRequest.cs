namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class TransferBalanceRequest
    {
        public string Token { get; internal set; }
        public object MobileNumber { get; internal set; }
        public object TransferBalance { get; internal set; }
        public object TransferCurrency { get; internal set; }
        public object Username { get; internal set; }
    }
}