namespace TickAndDash.Services.ServicesDtos
{
    public class DeviceInfo
    {
        public string UserAgent { get; set; }
        public string DeviceModel { get; set; }
        //public string DeviceName { get; set; }
        public string OSVersion { get; set; }
        public string Operation { get; set; }
        public string IP { get; set; }
    }

    public class IsAtuhDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
