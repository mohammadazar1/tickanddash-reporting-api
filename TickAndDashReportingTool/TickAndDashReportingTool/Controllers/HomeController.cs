using Microsoft.AspNetCore.Mvc;

namespace TickAndDashReportingTool.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Get()
        {
            return Ok(new 
            { 
                message = "Tick & Dash Reporting Tool API is running!",
                status = "OK",
                version = "1.0",
                timestamp = System.DateTime.UtcNow,
                endpoints = new
                {
                    swagger = "/swagger",
                    api = "/api",
                    api_v1 = "/api/v1"
                }
            });
        }
    }
}

