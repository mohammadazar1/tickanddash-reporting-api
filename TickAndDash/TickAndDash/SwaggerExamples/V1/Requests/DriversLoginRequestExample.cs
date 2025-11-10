using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Requests;

namespace TickAndDash.SwaggerExamples.V1.Requests
{
    public class DriversLoginRequestExample : IExamplesProvider<DriversLoginRequest>
    {
        public DriversLoginRequest GetExamples()
        {
            return new DriversLoginRequest
            {
                LicenseNumber = "123456",
                Password = "driver1",
                FCMToken = "cYV_LSoITQqD2QYoLrrb1B:APA91bH13UF8NSZkk0tOy7xBPPuD18n574uTuAJd1TQxs4VloXsRm98o22hQOooMAavHD7zLtWOrY1gGiVcDiUjsHzkBbvlZ2b2XaugQUrlDU0AmHV4Bc2FZ3jxPdrC-KwpJh27tnzeX"
            };
        }
    }
}
