using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickAndDash.Filters;
using TickAndDash.Installers.Interfaces;

namespace TickAndDash.Installers
{
    public class ControllersInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.IgnoreNullValues = true;
            }).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
              .AddDataAnnotationsLocalization(); ;

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            //services.AddSingleton<LanguageActionFilter>();
        }
    }
}
