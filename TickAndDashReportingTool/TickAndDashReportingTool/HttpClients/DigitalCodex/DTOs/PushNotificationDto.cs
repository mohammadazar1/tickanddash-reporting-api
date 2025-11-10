namespace TickAndDashReportingTool.HttpClients.DigitalCodex
{
    public class PushNotificationDto
    {
        public string to { get; set; }
        public Notification notification { get; set; }
        public Data data { get; set; }
    }

    public class Notification
    {
        public string body { get; set; }
        public string title { get; set; }
        //public string icon { get; set; }
        public string priority { get; set; } = "high";
        public string click_action { get; set; }
        public string category { get; set; }
        public string sound { get; set; }

    }
    public class Data
    {
        public string body { get; set; }
        public string title { get; set; }
        public string click_action { get; set; }
        public string category { get; set; }
        public string sound { get; set; }

    }

}