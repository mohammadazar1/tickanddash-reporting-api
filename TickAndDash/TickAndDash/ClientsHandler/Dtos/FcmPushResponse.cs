using System.Collections.Generic;

namespace TickAndDash.ClientsHandler.Dtos
{
    public class FcmPushResponse
    {
        public long Multicast_id { get; set; }
        public int Success { get; set; }
        public int Failure { get; set; }
        public List<Messages> Results { get; set; }
    }
    public class Messages
    {
        public string Message_id { get; set; }
    }

}
