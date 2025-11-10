using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TickAndDash.Installers.Interfaces;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;

namespace TickAndDash.Installers
{
    public class DALInstallers : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUsersDAL, UserDAL>();
            services.AddSingleton<IRidersDAL, RidersDAL>();
            services.AddSingleton<IDriversDAL, DriversDAL>();
            services.AddSingleton<ICityDAL, CityDAL>();
            services.AddSingleton<ITransItinerariesDAL, TransItinerariesDAL>();
            services.AddSingleton<IPickupStationsDAL, PickupStationsDAL>();
            services.AddSingleton<ISitesDAL, SitesDAL>();
            services.AddSingleton<ICarsQueueDAL, CarsQueueDAL>();
            services.AddSingleton<ICarsDAL, CarsDAL>();
            services.AddSingleton<IRidersQueueDAL, RidersQueueDAL>();
            services.AddSingleton<IUsersSessionsDAL, UsersSessionsDAL>();
            services.AddSingleton<ISystemConfigurationDAL, SystemConfigurationDAL>();
            services.AddSingleton<IBlacklistedUsersTokensDAL, BlacklistedUsersTokensDAL>();
            services.AddSingleton<ICarsTripsDAL, CarsTripsDAL>();
            services.AddSingleton<ITripsRidersDAL, TripsRidersDAL>();
            services.AddSingleton<IBlockedUsersDAL, BlockedUsersDAL>();
            services.AddSingleton<IRidersTicketsDAL, RidersTicketsDAL>();
            services.AddSingleton<IComplaintsDAL, ComplaintsDAL>();
            services.AddSingleton<IComplaintsTicketsDAL, ComplaintsTicketsDAL>();
            services.AddSingleton<IUserTransactionsDAL, UserTransactionsDAL>();
            services.AddSingleton<IMajorsMinorStationsDAL, MajorsMinorStationsDAL>();
            services.AddSingleton<ICallbacksDAL, CallbacksDAL>();
            services.AddSingleton<IComplaintTypesTranslationsDAL, ComplaintTypesTranslationsDAL>();
            services.AddSingleton<IComplaintsSubTypeTranslations, ComplaintsSubTypeTranslationsDAL>();
            services.AddSingleton<IPointOfSalesDAL, PointOfSalesDAL>();
            services.AddSingleton<IRefillRequestsDAL, RefillRequestsDAL>();
        }
    }
}
