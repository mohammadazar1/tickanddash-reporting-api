using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using TickAndDash.Controllers.V1.Responses;

namespace TickAndDash.SwaggerExamples.V1.Requests
{
    public class GetAllSitesCanVisitResponseExample : IExamplesProvider<GeneralResponse<List<GetAllSitesCanVisitResponse>>>
    {
        public GeneralResponse<List<GetAllSitesCanVisitResponse>> GetExamples()
        {
            return new GeneralResponse<List<GetAllSitesCanVisitResponse>>()
            {
                Success = true,
                Code = "Ok",
                Message = "تم معالجة الطلب بنجاح",
                Data = new List<GetAllSitesCanVisitResponse>()
                {
                    new GetAllSitesCanVisitResponse()
                    {
                        Id = 1,
                        Name = "بيرزيت"
                    }
                }
            };
        }
    }
}
