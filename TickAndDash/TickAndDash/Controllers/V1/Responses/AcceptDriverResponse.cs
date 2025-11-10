namespace TickAndDash.Controllers.V1.Responses
{
    public class AcceptDriverResponse
    {
        public string Ticket { get; set; }
    }

    public class GetRiderCancellationRecord
    {
        public int CancellationCount { get; set; }
    }
}
