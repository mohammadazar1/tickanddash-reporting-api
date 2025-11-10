using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashDAL.DAL;
using TickAndDashDAL.DAL.Interfaces;
using TickAndDashReportingTool.Installers.Interfaces;

namespace TickAndDashReportingTool.Installers
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
            services.AddSingleton<IAdminDAL, AdminDAL>();
            services.AddSingleton<IRidersTicketsDAL, RidersTicketsDAL>();
            services.AddSingleton<IUserTransactionDAL, UserTransactionDAL>();
            services.AddSingleton<IUserTransactionsDAL, UserTransactionsDAL>();
            services.AddSingleton<IMajorsMinorStationsDAL, MajorsMinorStationsDAL>();
            services.AddSingleton<IPointOfSalesDAL, PointOfSalesDAL>();
            services.AddSingleton<IPaymentsToDriversDAL, PaymentsToDriversDAL>();
        }
    }
}
