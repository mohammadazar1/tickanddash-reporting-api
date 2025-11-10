using System.Collections.Generic;

namespace TickAndDashSharedServices.ClientsHandler.Dtos
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
        public string error { get; set; }
    }



    //{"multicast_id":8711007400758543062,"success":0,"failure":1,"canonical_ids":0,"results":[{"error":"NotRegistered"}]}
}
