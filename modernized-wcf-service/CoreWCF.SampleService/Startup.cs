// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.SampleService.BL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WCF.SampleService.Behaviors.ErrorBehavior;
using WCF.SampleService.Behaviors.HeaderValidationBehavior;
using WCF.SampleService.Contracts;
using WCF.SampleService.Loggers;
using WCF.SampleService.Services;

namespace CoreWCF.SampleService
{
    public class Startup
    {
        private readonly IConfiguration m_configuration;

        public Startup(IConfiguration configuration)
        {
            m_configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();
            services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

            // Register dependencies
            services.AddSingleton<IFileLogger, FileLogger>(serviceProvider => new FileLogger(m_configuration["appSettings:logFilePath"]));
            services.AddSingleton<ISomeBL, SomeBL>();
            services.AddSingleton<CalculatorService>();
            services.AddSingleton<AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseServiceModel(builder =>
            {
                builder.AddService<AuthService>((serviceOptions) => { });
                builder.AddServiceEndpoint<AuthService, IAuthService>(new BasicHttpBinding(), "/Services/AuthService.svc");

                builder.AddService<CalculatorService>((serviceOptions) => { });
                builder.AddServiceEndpoint<CalculatorService, ICalculatorService>(new BasicHttpBinding(), "Services/CalculatorService.svc");

                builder.ConfigureAllServiceHostBase(serviceHost =>
                {
                    // Add service behaviors
                    var fileLogger = app.ApplicationServices.GetRequiredService<IFileLogger>();
                    serviceHost.Description.Behaviors.Add(new ErrorBehavior(typeof(GlobalErrorHandler), fileLogger));

                    // Add endpoint behaviors
                    serviceHost.Description.Endpoints.ToList().ForEach(endpoint =>
                    {
                        endpoint.EndpointBehaviors.Add(new HeaderValidationBehavior());
                    });

                    var debug = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();

                    if (debug == null)
                    {
                        serviceHost.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
                    }
                    else
                    {
                        debug.IncludeExceptionDetailInFaults = true;
                    }
                });
            });

            var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<ServiceMetadataBehavior>();
            serviceMetadataBehavior.HttpGetEnabled = true;
        }
    }
}