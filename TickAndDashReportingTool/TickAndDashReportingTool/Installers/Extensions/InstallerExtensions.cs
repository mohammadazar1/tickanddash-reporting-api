using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDashReportingTool.Installers.Interfaces;

namespace TickAndDashReportingTool.Installers.Extensions
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
                    typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

                foreach (var installer in installers)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Installing services from: {installer.GetType().Name}");
                        installer.InstallServices(services, configuration);
                        System.Diagnostics.Debug.WriteLine($"Successfully installed services from: {installer.GetType().Name}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error installing services from {installer.GetType().Name}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                        throw; // Re-throw to see which installer failed
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InstallServicesInAssembly: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
