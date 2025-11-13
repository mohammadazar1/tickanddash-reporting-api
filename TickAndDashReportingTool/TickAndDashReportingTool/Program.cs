using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TickAndDashReportingTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() // Read from Azure App Service Configuration
                .Build();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                // Log error if logging is configured
                System.Diagnostics.Debug.WriteLine($"Application failed to start: {ex.Message}");
                throw; // Re-throw to ensure Azure logs the error
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseWebRoot("wwwroot"); // Explicitly set wwwroot folder
                    // Azure App Service automatically sets the PORT environment variable
                    // Don't override it, let Azure handle it
                    // webBuilder.UseUrls() is not needed for Azure App Service
                });
    }
}
