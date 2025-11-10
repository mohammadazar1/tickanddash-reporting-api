namespace TickAndDashDAL.Models
{
    public class BlockedUsers
    {
        public int UserId { get; set; }
        public string BlockedDate { get; set; }
        public string UnblockedDate { get; set; }
        public bool IsBlokced { get; set; }

    }

}
