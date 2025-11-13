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
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionContext = context.Features.Get<IExceptionHandlerFeature>();
                    string message = "An error occurred while processing your request.";
                    string stackTrace = null;

                    // For HTML requests, return HTML error page
                    var acceptHeader = context.Request.Headers["Accept"].ToString();
                    bool isHtmlRequest = acceptHeader.Contains("text/html") || 
                                        context.Request.Path.Value?.EndsWith(".html") == true ||
                                        context.Request.Path.Value == "/";

                    if (exceptionContext != null && exceptionContext.Error != null)
                    {
                        var exception = exceptionContext.Error;
                        message = exception.Message ?? message;
                        stackTrace = env.IsDevelopment() ? exception.StackTrace : null;

                        if (exception is HttpStatusException httpStatusException)
                        {
                            context.Response.StatusCode = (int)httpStatusException.StatusCode;
                            message = httpStatusException.Message ?? message;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        }
                    }

                    if (isHtmlRequest)
                    {
                        context.Response.ContentType = "text/html";
                        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Error {context.Response.StatusCode}</title>
</head>
<body>
    <h1>Error {context.Response.StatusCode}</h1>
    <p>{System.Net.WebUtility.HtmlEncode(message)}</p>
    {(stackTrace != null ? $"<pre>{System.Net.WebUtility.HtmlEncode(stackTrace)}</pre>" : "")}
</body>
</html>";
                        await context.Response.WriteAsync(html);
                    }
                    else
                    {
                        context.Response.ContentType = "application/json";
                        var response = JsonConvert.SerializeObject(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Success = false,
                            Message = message,
                            StackTrace = stackTrace
                        });
                        await context.Response.WriteAsync(response);
                    }
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

            // Static files should be before routing
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
