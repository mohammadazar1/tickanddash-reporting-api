using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickAndDashReportingTool.Installers.Interfaces;

namespace TickAndDashReportingTool.Installers
{
    public class ControllersInstaller: IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.IgnoreNullValues = true;
                
            });
        }
    }
}
