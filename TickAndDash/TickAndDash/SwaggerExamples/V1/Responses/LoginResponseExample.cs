using Swashbuckle.AspNetCore.Filters;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;

namespace TickAndDash.SwaggerExamples.V1.Responses
{
    public class LoginResponseExample : IExamplesProvider<GeneralResponse<LoginResponse>>
    {

        GeneralResponse<LoginResponse> IExamplesProvider<GeneralResponse<LoginResponse>>.GetExamples()
        {
            return new GeneralResponse<LoginResponse>()
            {
                Success = true,
                Code = Generalcodes.Ok.ToString(),
                Message = "تم تسجيل الدخول بنجاح",
                Data = new LoginResponse()
                {
                    //ExpirationMinutes = 30,
                    Token = "eyJhbGciOiJIafsdgUzI1NeyJuYW1laWQasdfiOiI5NzI1OTg2NjA1MDMiLCJqdGkiOiI4NzM0MjBiNS00MzIyLTQ1YWEtYjkzMS0wY2I4NjcxZGFhYjEiLCJodHRwOi8vc2NoZW1hcy54basWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6asdWyI5NDg3IiwiTW96aWxsYS81LjAgKFdpasdbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNsdagrbykgQ2hyb21lLzg1LjAuNDE4My4xMjEgU2FmYXJpLzUzNy4zNAiJdLCiIsInR5cCI6IkpXVCJ9.",
                }
            };
        }
    }
}
