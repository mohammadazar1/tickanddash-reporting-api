using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TickAndDashReportingTool.Installers.Interfaces
{
    interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}
