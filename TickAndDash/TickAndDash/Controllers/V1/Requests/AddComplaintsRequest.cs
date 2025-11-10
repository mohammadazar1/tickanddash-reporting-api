namespace TickAndDash.Controllers.V1.Requests
{
    public class AddComplaintsRequest
    {
        /// <summary>
        /// ComplaintText
        /// </summary>
        /// <example>Test</example>
        public string ComplaintText { get; set; }

        public int ItineraryId { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public string Title { get; set; }
    }
}
