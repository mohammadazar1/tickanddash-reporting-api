using Dapper.Contrib.Extensions;
using System;

namespace TickAndDashDAL.Models
{
    [Dapper.Contrib.Extensions.Table("ComplaintsStory")]
    public class ComplaintsTickets
    {
        public int Id { get; set; }

        public int ComplaintId { get; set; }

        [Computed]
        public Complaint Complaint {get; set;}
        
        public int UserId { get; set; }
        
        [Computed]
        public User User { get; set; }

        public string ComplaintReply { get; set; }

        public DateTime CreationDate { get; set; }

    }

}
