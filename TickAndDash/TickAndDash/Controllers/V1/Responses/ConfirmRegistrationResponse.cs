using System;

namespace TickAndDash.Controllers.V1.Responses
{
    public class ConfirmRegistrationResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class SubscribeResponse
    {
        public bool Subscribed { get; set; }
        public string Status { get; set; }
        public DateTime ActiveSubscriptionPeriod { get; set; }
    }

    public class IsSubscribeResponse
    {
        public string Status { get; set; }
        public DateTime ActiveSubscriptionPeriod { get; set; }
    }


    public class RefillingCountResponse
    {
        public int RefillingCount { get; set; }
    }
}
