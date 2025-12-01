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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });
            services.InstallServicesInAssembly(Configuration);
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
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
                    var exception = exceptionContext.Error;
                    var message = "Internal server Error";

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    if (exception != null)
                    {
                        var configuration = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json")
                           .Build();
                    }

                    if (exception is HttpStatusException httpStatusException)
                    {
                        context.Response.StatusCode = (int)httpStatusException.StatusCode;
                        message = httpStatusException.Message;
                    }

                    var reposne = JsonConvert.SerializeObject(new
                    {
                        StatusCode = context.Response.StatusCode,
                        Success = false,
                        Message = message,
                    });

                    await context.Response.WriteAsync(reposne);
                });
            });

            // Configure Swagger with safe defaults in case configuration section is missing
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            // Fallback defaults so that Swagger does not crash app startup if config is missing
            swaggerOptions.JsonRoute ??= "swagger/{documentName}/swagger.json";
            swaggerOptions.UiEndpoint ??= "/swagger/v1/swagger.json";
            swaggerOptions.Description ??= "Tick & Dash Reporting API v1";

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
            });

            app.UseStaticFiles();

            // Add Content Security Policy to prevent malicious script injection
            app.Use(async (context, next) =>
            {
                // Only apply CSP to HTML pages, not API endpoints
                if (context.Request.Path.StartsWithSegments("/api") == false)
                {
                    context.Response.Headers.Add("Content-Security-Policy", 
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://fonts.googleapis.com https://fonts.gstatic.com; " +
                        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://fonts.gstatic.com; " +
                        "font-src 'self' https://fonts.gstatic.com data:; " +
                        "img-src 'self' data: https:; " +
                        "connect-src 'self' https://tickanddash-hmexcjh6ewescwa2.canadacentral-01.azurewebsites.net https://tickanddash-backend-api-cmghdnbbedfzapfd.canadacentral-01.azurewebsites.net; " +
                        "frame-ancestors 'none'; " +
                        "base-uri 'self'; " +
                        "form-action 'self'");
                }
                await next();
            });

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
