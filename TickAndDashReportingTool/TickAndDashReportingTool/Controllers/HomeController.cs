using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace TickAndDashReportingTool.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        [Produces("text/html")]
        public IActionResult Get() => ServeSpa();

        [HttpGet("/login")]
        [HttpGet("/login.html")]
        [Produces("text/html")]
        public IActionResult LoginPage() => ServeSpa();

        private IActionResult ServeSpa()
        {
            try
            {
                var indexPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
                if (System.IO.File.Exists(indexPath))
                {
                    return PhysicalFile(indexPath, "text/html");
                }

                return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Tick & Dash</title>
</head>
<body>
    <h1>Application not yet built</h1>
    <p>The Angular admin dashboard has not been published to wwwroot.</p>
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
    <p>Error loading application shell: {System.Net.WebUtility.HtmlEncode(ex.Message)}</p>
</body>
</html>", "text/html");
            }
        }
    }
}

