using System.Collections.Generic;

namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class FcmPushResponse
    {
        public long Multicast_id { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
        public List<Messages> Results { get; set; }
    }
}