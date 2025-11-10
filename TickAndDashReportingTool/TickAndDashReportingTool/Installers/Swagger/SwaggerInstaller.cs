using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TickAndDashReportingTool.Installers.Interfaces;
using TickAndDashReportingTool.Installers.Swagger;

namespace TickAndDashReportingTool.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "Tick And Dash", Version = "v1" });
                x.ExampleFilters();
                x.OperationFilter<CustomHeaderSwaggerAttribute>("1.0", "Used For API Versioning");
                
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme{Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }}, new List<string>()}
                });

                
            });


            services.AddSwaggerExamples();

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
        }
    }
}
