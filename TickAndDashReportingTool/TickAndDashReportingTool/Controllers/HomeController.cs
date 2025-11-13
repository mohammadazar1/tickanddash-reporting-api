using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TickAndDashReportingTool.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        [Produces("text/html")]
        public IActionResult Get()
        {
            try
            {
                // Simple redirect - let static file middleware handle /login.html
                return Redirect("/login.html");
            }
            catch (Exception ex)
            {
                // Return error page if redirect fails
                return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Error</title>
</head>
<body>
    <h1>Error</h1>
    <p>Error redirecting to login page: {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
</body>
</html>", "text/html");
            }
        }

        [HttpGet("/login")]
        [HttpGet("/login.html")]
        [Produces("text/html")]
        public IActionResult LoginPage()
        {
            try
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
            catch (Exception ex)
            {
                return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Error</title>
</head>
<body>
    <h1>Error</h1>
    <p>Error loading login page: {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
</body>
</html>", "text/html");
            }
        }
    }
}

