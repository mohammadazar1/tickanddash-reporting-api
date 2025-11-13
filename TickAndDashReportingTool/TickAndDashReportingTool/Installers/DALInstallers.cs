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
            // Register IConfiguration so DAL classes can access it
            services.AddSingleton<IConfiguration>(configuration);
            
            services.AddSingleton<IUsersDAL>(sp => new UserDAL(configuration));
            services.AddSingleton<IRidersDAL>(sp => new RidersDAL(configuration));
            services.AddSingleton<IDriversDAL>(sp => new DriversDAL(configuration));
            services.AddSingleton<ICityDAL>(sp => new CityDAL(configuration));
            services.AddSingleton<ITransItinerariesDAL>(sp => new TransItinerariesDAL(configuration));
            services.AddSingleton<IPickupStationsDAL>(sp => new PickupStationsDAL(configuration));
            services.AddSingleton<ISitesDAL>(sp => new SitesDAL(configuration));
            services.AddSingleton<ICarsQueueDAL>(sp => new CarsQueueDAL(configuration));
            services.AddSingleton<ICarsDAL>(sp => new CarsDAL(configuration));
            services.AddSingleton<IRidersQueueDAL>(sp => new RidersQueueDAL(configuration));
            services.AddSingleton<IUsersSessionsDAL>(sp => new UsersSessionsDAL(configuration));
            services.AddSingleton<ISystemConfigurationDAL>(sp => new SystemConfigurationDAL(configuration));
            services.AddSingleton<IBlacklistedUsersTokensDAL>(sp => new BlacklistedUsersTokensDAL(configuration));
            services.AddSingleton<ICarsTripsDAL>(sp => new CarsTripsDAL(configuration));
            services.AddSingleton<ITripsRidersDAL>(sp => new TripsRidersDAL(configuration));
            services.AddSingleton<IAdminDAL>(sp => new AdminDAL(configuration));
            services.AddSingleton<IRidersTicketsDAL>(sp => new RidersTicketsDAL(configuration));
            services.AddSingleton<IUserTransactionDAL>(sp => new UserTransactionDAL(configuration));
            services.AddSingleton<IUserTransactionsDAL>(sp => new UserTransactionsDAL(configuration));
            services.AddSingleton<IMajorsMinorStationsDAL>(sp => new MajorsMinorStationsDAL(configuration));
            services.AddSingleton<IPointOfSalesDAL>(sp => new PointOfSalesDAL(configuration));
            services.AddSingleton<IPaymentsToDriversDAL>(sp => new PaymentsToDriversDAL(configuration));
        }
    }
}
