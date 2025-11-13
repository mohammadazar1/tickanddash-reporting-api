using Microsoft.AspNetCore.Mvc;

namespace TickAndDashReportingTool.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Get()
        {
            // Redirect to login page
            return Redirect("/login.html");
        }

        [HttpGet("/login")]
        public IActionResult LoginPage()
        {
            // Return login.html
            return PhysicalFile(
                System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(),
                    "wwwroot", "login.html"),
                "text/html");
        }
    }
}

