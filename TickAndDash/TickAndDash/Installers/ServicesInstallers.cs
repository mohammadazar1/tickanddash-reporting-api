using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickAndDash.Installers.Interfaces;
using TickAndDash.Services;
using TickAndDash.Services.Interfaces;

namespace TickAndDash.Installers
{
    public class ServicesInstallers : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ISMSService, SMSService>();
            services.AddSingleton<IUsersService, UsersService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<ISitesServices, SitesServices>();
            services.AddSingleton<IPickupStationsService, PickupStationsService>();
            services.AddSingleton<ITransItinerariesService, TransItinerariesService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICarService, CarService>();
            services.AddSingleton<IQueuesService, QueuesService>();
            services.AddSingleton<ISystemConfigurationService, SystemConfigurationService>();
            services.AddSingleton<IBlacklistedService, BlacklistedService>();
            services.AddSingleton<ITripsService, TripsService>();
            services.AddSingleton<ICarsQueueService, CarsQueueService>();
            services.AddSingleton<IRidersQueueService, RidersQueueService>();
            services.AddSingleton<IRidersTicketsServices, RidersTicketsServices>();
            services.AddSingleton<IComplaintsServices, ComplaintsServices>();
            services.AddSingleton<IWalletService, WalletService>();
            services.AddSingleton<IUserTransactionsService, UserTransactionsService>();
            services.AddSingleton<ICallbackServices, CallbackServices>();
            services.AddSingleton<IPointOfSalesServices, PointOfSalesServices>();
            services.AddSingleton<IRefillRequestsService, RefillRequestsService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ICarsTripsService, CarsTripsService>();
        }
    }
}
