using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TickAndDash.Installers.Interfaces
{
    interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}
