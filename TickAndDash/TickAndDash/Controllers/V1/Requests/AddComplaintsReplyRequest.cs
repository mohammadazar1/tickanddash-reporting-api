namespace TickAndDash.Controllers.V1.Requests
{
    public class AddComplaintsReplyRequest
    {
        /// <summary>
        /// ComplaintReply
        /// </summary>
        /// <example>Test</example>
        public string ComplaintReply { get; set; }

        /// <summary>
        /// ComplaintId
        /// </summary>
        public int ComplaintId { get; set; }
    }
}
