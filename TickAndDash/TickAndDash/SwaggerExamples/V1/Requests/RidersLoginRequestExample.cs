using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Controllers.V1.Requests;

namespace TickAndDash.SwaggerExamples.V1.Requests
{
    public class RidersLoginRequestExample : IExamplesProvider<RidersLoginRequest>
    {
        public RidersLoginRequest GetExamples()
        {
            return new RidersLoginRequest()
            {
                MobileNumber = "0598660503",
                Pincode = 1234,
                MobileOperatingSystem = "iOS",
                FCMToken = "cYV_LSoITQqD2QYoLrrb1B:APA91bH13UF8NSZkk0tOy7xBPPuD18n574uTuAJd1TQxs4VloXsRm98o22hQOooMAavHD7zLtWOrY1gGiVcDiUjsHzkBbvlZ2b2XaugQUrlDU0AmHV4Bc2FZ3jxPdrC-KwpJh27tnzeX"
            };
        }
    }
}
