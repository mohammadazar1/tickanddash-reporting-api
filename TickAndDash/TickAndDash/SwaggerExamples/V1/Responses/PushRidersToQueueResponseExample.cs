using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class PushRidersToQueueResponseExample : IExamplesProvider<GeneralResponse<PushRidersToQueueResponse>>
    {
        public GeneralResponse<PushRidersToQueueResponse> GetExamples()
        {
            return new GeneralResponse<PushRidersToQueueResponse>
            {
                Success = true,
                Message = "تم معالجة الطلب بنجاح",
                Code = Generalcodes.Ok.ToString(),
                Data = new PushRidersToQueueResponse()
                {
                    Turn = 5
                }
            };
        }
    }


    //public class PushDriversToCarQueueResponseExample : IExamplesProvider<GeneralResponse<PushDriversToCarQueueResponse>>
    //{
    //    public GeneralResponse<PushDriversToCarQueueResponse> GetExamples()
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
