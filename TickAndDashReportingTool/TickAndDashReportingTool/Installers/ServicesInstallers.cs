using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashReportingTool.HttpClients.DigitalCodex;
using TickAndDashReportingTool.HttpClients.DigitalCodex.Interfaces;
using TickAndDashReportingTool.Installers.Interfaces;
using TickAndDashReportingTool.Services;
using TickAndDashReportingTool.Services.Interfaces;

namespace TickAndDashReportingTool.Installers
{
    public class ServicesInstallers : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IDigitalCodexClient, DigitalCodexClient>();
            services.AddHttpClient<IFCMHttpClient, FCMHttpClient>();

            services.AddSingleton<IUsersService, UsersService>();
            services.AddSingleton<IDriversService, DriversService>();
            services.AddSingleton<ICarsService, CarsService>();
            services.AddSingleton<ISystemConfigurationService, SystemConfigurationService>();
            services.AddSingleton<ISitesService, SitesService>();
            services.AddSingleton<ICarsQueuesService, CarsQueuesService>();
            services.AddSingleton<IRiderTripsService, RiderTripsService>();
            services.AddSingleton<IRiderQueueService, RiderQueueService>();
            services.AddSingleton<ICarsTripsService, CarsTripsService>();
            services.AddSingleton<IItinerariesService, ItinerariesService>();
            services.AddSingleton<ITicketService, TicketService>();
            services.AddSingleton<IFinancialsService, FinancialsService>();
            services.AddSingleton<Services.IPickupStationsService, PickupStationsService>();
            services.AddSingleton<IPosService, PosService>();
            services.AddSingleton<IPaymentsToDriversService, PaymentsToDriversService>();
            services.AddSingleton<IUserTransactionsService, UserTransactionsService>();
            services.AddSingleton<ISMSService, SMSService>();
        }
    }
}
