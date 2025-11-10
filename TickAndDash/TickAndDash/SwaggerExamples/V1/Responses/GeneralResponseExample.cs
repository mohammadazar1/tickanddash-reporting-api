using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class GeneralResponseExample : IExamplesProvider<GeneralResponse<object>>
    {
        public GeneralResponse<object> GetExamples()
        {
            return new GeneralResponse<object>()
            {
                Success = true,
                Message = "تم معالجة الطلب بنجاح",
                Code = Generalcodes.Ok.ToString(),
                Data = null
            };
        }
    }
}
