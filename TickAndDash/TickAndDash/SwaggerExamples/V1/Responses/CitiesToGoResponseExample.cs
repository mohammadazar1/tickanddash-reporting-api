using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class CitiesToGoResponseExample : IExamplesProvider<GeneralResponse<List<GetActiveSitesResponse>>>
    {
        //public GeneralResponse<GetActiveSitesResponse> GetExamples()
        //{
        //    return new GeneralResponse<GetActiveSitesResponse>()
        //    {
        //        Code = Generalcodes.Ok.ToString(),
        //        Message = "تم معالجة الطلب بنجاح",
        //        Success = true,
        //        Data = new GetActiveSitesResponse
        //        {
        //            SitesToGo = new List<SitesToGo> {
        //                new SitesToGo()
        //                {
        //                    Id =1,
        //                    Name = "Ramallah"
        //                }
        //            },
        //        }
        //    };
        //}
        public GeneralResponse<List<GetActiveSitesResponse>> GetExamples()
        {
            return new GeneralResponse<List<GetActiveSitesResponse>>
            {
                Code = Generalcodes.Ok.ToString(),
                Message = "تم معالجة الطلب بنجاح",
                Success = true,
                Data = new List<GetActiveSitesResponse>
                {
                    new GetActiveSitesResponse
                    {
                        Id = 1,
                        Name = "Ramallah"
                    }
                }
            };
        }
    }
}
