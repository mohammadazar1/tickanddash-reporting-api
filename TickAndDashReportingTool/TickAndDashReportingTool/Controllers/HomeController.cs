using Microsoft.AspNetCore.Mvc;
using System;

namespace TickAndDashReportingTool.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Get()
        {
            // Simple redirect - let static file middleware handle /login.html
            return Redirect("/login.html");
        }

        [HttpGet("/login")]
        [HttpGet("/login.html")]
        public IActionResult LoginPage()
        {
            // This should be handled by static file middleware
            // If we reach here, return a simple message
            return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Login</title>
</head>
<body>
    <h1>Login Page</h1>
    <p>If you see this message, the login.html file is not being served by static file middleware.</p>
    <p>Please ensure login.html exists in the wwwroot folder.</p>
</body>
</html>", "text/html");
        }
    }
}

