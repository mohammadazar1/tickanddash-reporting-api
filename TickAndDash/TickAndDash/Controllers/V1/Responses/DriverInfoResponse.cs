namespace TickAndDash.Controllers.V1.Responses
{
    public class DriverInfoResponse
    {
        public string MobileNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Name { get; set; } = " ";
        public string Address { get; set; } = " ";
        public DriverCarInfo CarInfoResponse { get; set; } = new DriverCarInfo();
        public DriverTransItineraryInfo TransItineraryResponse { get; set; } = new DriverTransItineraryInfo();
    }

    public class DriverCarInfo
    {
        public string RegistrationPlate { get; set; }
        public string Model { get; set; } = " ";
        public string ModelYear { get; set; } = " ";
        public int SeatCount { get; set; }
        public string CarCode { get; set; }
    }

    public class DriverTransItineraryInfo
    {
        public string Name { get; set; }
        public string Description { get; set; } = " ";
    }


}
