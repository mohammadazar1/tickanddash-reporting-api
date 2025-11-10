using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TickAndDash.Services.ServicesDtos
{
    public class OutRespons
    {
        public long ID { get; set; }
        public int UserServiceId { get; set; }
        public string Lang { get; set; }
        public int ShortCode { get; set; }
        public byte IsHex { get; set; }
        public string Msg { get; set; }
        public string MobileNumber { get; set; }
        public int SenderId { get; set; }
        public DateTime CreationDate { get; set; }
        public int Sent { get; set; }
        public int IsPIN { get; set; }
        public int IsPinSent { get; set; }
        public string Status { get; set; }
        public int IsStatusDelivered { get; set; }
    }
}
