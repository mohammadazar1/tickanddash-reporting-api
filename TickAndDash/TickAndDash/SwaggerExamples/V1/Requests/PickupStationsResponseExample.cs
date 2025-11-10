using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Requests
{
    public class PickupStationsResponseExample : IExamplesProvider<GeneralResponse<List<PickupStationsResponse>>>
    {
        public GeneralResponse<List<PickupStationsResponse>> GetExamples()
        {
            return new GeneralResponse<List<PickupStationsResponse>>()
            {
                Success = true,
                Code = Generalcodes.Ok.ToString(),
                Message = "تم معالجة الطلب بنجاح",
                Data = new List<PickupStationsResponse>
                {
                   new PickupStationsResponse
                   {
                       PikcupId = 3,
                       Latitude = 31.909190M,
                       Longitude = 35.207891M,
                       SiteId = 4,
                       SiteName = "رام الله",
                       ItineraryId = 2,
                       ItineraryName = "رام الله-بيرزيت"
                   }
                }
              
            };
        }
    }
}
