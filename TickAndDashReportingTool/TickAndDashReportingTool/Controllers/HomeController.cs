using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;

namespace TickAndDashReportingTool.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpGet("/")]
        public IActionResult Get()
        {
            try
            {
                // Try to find login.html in various possible locations
                var possiblePaths = new[]
                {
                    Path.Combine(_env.WebRootPath ?? "", "login.html"),
                    Path.Combine(_env.ContentRootPath, "wwwroot", "login.html"),
                    Path.Combine(_env.WebRootPath ?? "", "wwwroot", "login.html"),
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "login.html"),
                };

                string filePath = null;
                foreach (var path in possiblePaths)
                {
                    if (!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
                    {
                        filePath = path;
                        break;
                    }
                }

                if (filePath != null && System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "text/html");
                }

                // If file not found, return simple HTML redirect
                var redirectHtml = @"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""refresh"" content=""0;url=/login.html"">
    <title>Redirecting...</title>
</head>
<body>
    <p>Redirecting to login page... <a href=""/login.html"">Click here if not redirected</a></p>
</body>
</html>";
                return Content(redirectHtml, "text/html");
            }
            catch (Exception ex)
            {
                // Return error page with details for debugging
                var errorHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Error</title>
</head>
<body>
    <h1>Error Loading Login Page</h1>
    <p>Error: {ex.Message}</p>
    <p>WebRootPath: {_env?.WebRootPath ?? "null"}</p>
    <p>ContentRootPath: {_env?.ContentRootPath ?? "null"}</p>
    <p>Current Directory: {Directory.GetCurrentDirectory()}</p>
</body>
</html>";
                return Content(errorHtml, "text/html");
            }
        }

        [HttpGet("/login")]
        [HttpGet("/login.html")]
        public IActionResult LoginPage()
        {
            try
            {
                // Try to find login.html in various possible locations
                var possiblePaths = new[]
                {
                    Path.Combine(_env.WebRootPath ?? "", "login.html"),
                    Path.Combine(_env.ContentRootPath, "wwwroot", "login.html"),
                    Path.Combine(_env.WebRootPath ?? "", "wwwroot", "login.html"),
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "login.html"),
                };

                string filePath = null;
                foreach (var path in possiblePaths)
                {
                    if (!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
                    {
                        filePath = path;
                        break;
                    }
                }

                if (filePath != null && System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "text/html");
                }

                return NotFound("login.html file not found in wwwroot folder");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading login page: {ex.Message}");
            }
        }
    }
}

