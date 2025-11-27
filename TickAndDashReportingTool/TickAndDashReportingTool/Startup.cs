using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using TickAndDash.Controllers.V1.Responses;
using TickAndDash.Enums;
using TickAndDash.Installers.Interfaces.Extensions;
using TickAndDash.Options;
using TickAndDash.Services;

namespace TickAndDash
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
            services.InstallServicesInAssembly(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUsersService usersService, ILoggerFactory loggerFactory)
        {

            //loggerFactory.AddNLog();
            loggerFactory.AddSerilog();
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseRequestLocalization(options =>
            //{
            //    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ar");
            //    options.SupportedCultures = cultures;
            //    options.SupportedUICultures = cultures;
            //});

            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    Guid errorId = Guid.NewGuid();

                    var x = exceptionHandlerFeature.Error.Message;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;

                    if (exceptionHandlerFeature != null)
                    {
                        //var configuration = new ConfigurationBuilder()
                        //   .AddJsonFile("appsettings.json")
                        //   .Build();

                        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger<Startup>();
                        logger.LogInformation($"{x.ToString()}");
                        //logger.LogCritical("{ErrorId}: {StatusCode}", "{Error}", "{ErrorMessage}",
                        //   errorId,
                        //    500, exceptionHandlerFeature.Error
                        //    , exceptionHandlerFeature.Error.Message);
                        //Log.ForContext("ErrorId", errorId).ForContext("StatusCode", 500)
                        //.ForContext("Error", exceptionHandlerFeature.Error).
                        //ForContext("Message", exceptionHandlerFeature.Error.Message).
                        //Fatal("An Global Error Occurred");
                    }

                    var response = JsonConvert.SerializeObject(new GeneralResponse2<object>
                    {
                        //ErrorId = errorId,
                        success = false,
                        code = Generalcodes.Internal.ToString(),
                        message = x.ToString()
                    });


                    await context.Response.WriteAsync(response);
                });


            });

            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);

            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            //var cultures = new List<CultureInfo> {
            //    new CultureInfo("ar"),
            //    new CultureInfo("en")
            //};
            //var requestLocalizationOptions = new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ar"),
            //    SupportedCultures =  new List<CultureInfo> { new CultureInfo("en") },//cultures,
            //    SupportedUICultures = cultures/* new List<CultureInfo> { new CultureInfo("en") }*/
            //};
            //requestLocalizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async httpContext =>
            //{
            //    var user = httpContext.User as ClaimsPrincipal;
            //    int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
            //    string language = await usersService.GetUserLanguageAsync(userId) ?? "ar";
            //    httpContext.Request.Headers.Add("Content-Language", language);
            //    return new ProviderCultureResult(language);
            //}));
            //app.UseRequestLocalization(requestLocalizationOptions);
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
