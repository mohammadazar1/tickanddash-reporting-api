using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Net;
using TickAndDashReportingTool.Exceptions;
using TickAndDashReportingTool.Installers.Extensions;
using TickAndDashReportingTool.Options;

namespace TickAndDashReportingTool
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.InstallServicesInAssembly(Configuration);
            }
            catch (Exception ex)
            {
                // Log the error - this will be visible in Azure logs
                System.Diagnostics.Debug.WriteLine($"Error in ConfigureServices: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                throw; // Re-throw to prevent silent failures
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionContext = context.Features.Get<IExceptionHandlerFeature>();
                    string message = "An error occurred while processing your request.";

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    if (exceptionContext != null && exceptionContext.Error != null)
                    {
                        var exception = exceptionContext.Error;

                        if (exception is HttpStatusException httpStatusException)
                        {
                            context.Response.StatusCode = (int)httpStatusException.StatusCode;
                            message = httpStatusException.Message ?? message;
                        }
                        else
                        {
                            message = exception.Message ?? message;
                        }
                    }

                    var response = JsonConvert.SerializeObject(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Success = false,
                        Message = message,
                    });

                    await context.Response.WriteAsync(response);
                });
            });

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            // Use default values if SwaggerOptions are not configured
            // Swagger document name is "v1" (from SwaggerInstaller)
            var jsonRoute = swaggerOptions.JsonRoute ?? "swagger/{documentName}/swagger.json";
            var uiEndpoint = swaggerOptions.UiEndpoint ?? "v1/swagger.json";
            var description = swaggerOptions.Description ?? "Tick&Dash Reporting Tool APIs";

            app.UseSwagger(option => { option.RouteTemplate = jsonRoute; });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(uiEndpoint, description);
            });

            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
