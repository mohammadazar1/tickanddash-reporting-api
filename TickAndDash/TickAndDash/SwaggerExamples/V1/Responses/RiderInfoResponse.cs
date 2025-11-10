using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class RiderInfoResponseExample : IExamplesProvider<GeneralResponse<RiderInfoResponse>>
    {
        public GeneralResponse<RiderInfoResponse> GetExamples()
        {
            return new GeneralResponse<RiderInfoResponse>()
            {
                Success = true,
                Message = "تم معالجة الطلب بنجاح",
                Code = Generalcodes.Ok.ToString(),
                Data = new RiderInfoResponse()
                {
                    Name = "mohammad",
                    Gender = 'M'
                }
            };
        }
    }
}
