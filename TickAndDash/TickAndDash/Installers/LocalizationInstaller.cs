using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using TickAndDash.Enums;
using TickAndDash.Installers.Interfaces;
using TickAndDash.Services;

namespace TickAndDash.Installers
{
    public class LocalizationInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<IUsersService, UsersService>();

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            /*To add or remove localization providers, use the RequestLocalizationOptions */
            services.Configure<RequestLocalizationOptions>(options =>
            {
                /* Culture is used for number and date formats,
                A UI Culture is used for reading culture-specific data from the resource files.
                */
                List<CultureInfo> supportedUICultures = new List<CultureInfo>
                    {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar"),
                    };

                List<CultureInfo> supportedCultures = new List<CultureInfo>
                        {
                        new CultureInfo("en-US"),
                        };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedUICultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async httpContext =>
                {
                    var user = httpContext.User as ClaimsPrincipal;
                    int.TryParse(user.FindFirstValue(ClaimsEnum.UserId.ToString()), out int userId);
                    
                    var sp = services.BuildServiceProvider();
                    var usersService = sp.GetService<IUsersService>();
                    
                    string language = await usersService.GetUserLanguageAsync(userId) ?? "ar";
                    httpContext.Request.Headers.Add("Content-Language", language);
                    
                    //Write your code here
                    return new ProviderCultureResult(language);
                }));
            });
        }
    }
}
