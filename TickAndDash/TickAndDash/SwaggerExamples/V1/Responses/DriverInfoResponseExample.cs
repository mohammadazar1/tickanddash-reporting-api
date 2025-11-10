using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Responses;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class DriverInfoResponseExample : IExamplesProvider<GeneralResponse<DriverInfoResponse>>
    {
        public GeneralResponse<DriverInfoResponse> GetExamples()
        {
            return new GeneralResponse<DriverInfoResponse>
            {
                Success = true,
                Message = "تم معالجة الطلب بنجاح",
                Code = "Ok",
                Data = new DriverInfoResponse()
                {
                    LicenseNumber = "123456",
                    Name = "Driver1",
                    CarInfoResponse = new DriverCarInfo()
                    {
                        Model = "Audi",
                        ModelYear = "2020",
                        RegistrationPlate = "1-2-3",
                    },
                    Address = "رام الله",
                    TransItineraryResponse = new DriverTransItineraryInfo
                    {
                        Description = "بيرزيت-أبوقش-سردا-رام الله",
                        Name = "رام الله-بيرزيت"
                    }
                }
            };
        }
    }
}
