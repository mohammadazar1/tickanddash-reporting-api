using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickAndDash.ClientsHandler;
using TickAndDash.ClientsHandler.Interfaces;
using TickAndDash.HttpClients;
using TickAndDash.HttpClients.GeoClients;
using TickAndDash.HttpClients.GeoClients.Interfaces;
using TickAndDash.HttpClients.Interfaces;
using TickAndDash.Installers.Interfaces;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;

namespace TickAndDash.Installers
{
    public class HttpClientsInstallers : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IGeocodingClient, GeocodingClient>();
            services.AddHttpClient<IFCMHttpClient, FCMHttpClient>();
            services.AddHttpClient<IDigitalCodexClient, DigitalCodexClient>();
            services.AddHttpClient<IBulkSMSService, BulkSMSService>();

        }
    }
}
