namespace TickAndDash.Controllers.V1.Responses
{
    public class RiderInfoResponse
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public char? Gender { get; set; } = ' ';
    }


    public class GetLanguageInfoResponse
    {
        public string Language { get; set; }
    }
}
