using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TickAndDashReportingTool.Installers.Extensions;
using TickAndDashReportingTool.Options;

namespace TickAndDashReportingTool
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Logging
            Log.Information("Configuring services...");

            // Register all installers automatically
            services.InstallServicesInAssembly(Configuration);

            // Bind app settings
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Information("Configuring HTTP pipeline...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Security headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

                await next.Invoke();
            });

            // Static Files
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Log.Information("Startup pipeline ready ✔");
        }
    }
}
