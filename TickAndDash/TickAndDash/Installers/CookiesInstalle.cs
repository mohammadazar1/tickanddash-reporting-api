using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TickAndDash.Installers.Interfaces;

namespace TickAndDash.Installers
{
    //public class CookiesInstalle : IInstaller
    //{
    //    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.Configure<CookiePolicyOptions>(options =>
    //        {
    //            //options.CheckConsentNeeded = context => true;
    //            options.MinimumSameSitePolicy = SameSiteMode.Lax;
    //            options.HttpOnly = HttpOnlyPolicy.Always;
    //            //options.Secure = CookieSecurePolicy.Always;
    //            // you can add more options here and they will be applied to all cookies (middleware and manually created cookies)
    //        });
    //    }
    //}
}
