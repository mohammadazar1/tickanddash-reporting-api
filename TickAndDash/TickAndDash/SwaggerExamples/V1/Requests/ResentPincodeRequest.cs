using Swashbuckle.AspNetCore.Filters;
using System;
using TickAndDash.Controllers.V1.Requests;

namespace TickAndDash.SwaggerExamples.V1.Requests
{
    public class ResentPincodeRequestExample : IExamplesProvider<ResentPincodeRequest>
    {
        public ResentPincodeRequest GetExamples()
        {
            return new ResentPincodeRequest()
            {
                MobileNumber = "0598660503"
            };
        }
    }

    public class PushRidersToQueueRequestExample : IExamplesProvider<PushRidersToQueueRequest>
    {
        public PushRidersToQueueRequest GetExamples()
        {
            return new PushRidersToQueueRequest()
            {
                PickupStationId = 3,
                ReservationDate = DateTime.Now.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss"),
                CountOfSeats = 1
            };
        }
    }
}
