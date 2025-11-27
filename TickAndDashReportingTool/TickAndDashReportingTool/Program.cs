using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace TickAndDashReportingTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Console() // For local debugging
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // For Azure/Kudu logs
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting TickAndDash Reporting Tool API...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "API crashed on startup!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // 🔥 Activating Serilog
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
