namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class DigitalCodexRegisterUserResponse
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool IsAutherized { get; set; }
    }
}