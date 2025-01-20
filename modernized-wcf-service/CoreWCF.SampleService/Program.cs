// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.SampleService.BL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WCF.SampleService.Behaviors.ErrorBehavior;
using WCF.SampleService.Behaviors.HeaderValidationBehavior;
using WCF.SampleService.Contracts;
using WCF.SampleService.Loggers;
using WCF.SampleService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add WSDL support
builder.Services.AddServiceModelServices().AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// Register dependencies
builder.Services.AddSingleton<IFileLogger, FileLogger>(serviceProvider => new FileLogger(builder.Configuration["appSettings:logFilePath"]));
builder.Services.AddSingleton<ISomeBL, SomeBL>();
builder.Services.AddSingleton<CalculatorService>();
builder.Services.AddSingleton<AuthService>();

var app = builder.Build();

app.UseServiceModel(builder =>
{
    builder.AddService<AuthService>((serviceOptions) => { })
    .AddServiceEndpoint<AuthService, IAuthService>(new BasicHttpBinding(), "/Services/AuthService.svc");

    builder.AddService<CalculatorService>((serviceOptions) => { })
    .AddServiceEndpoint<CalculatorService, ICalculatorService>(new BasicHttpBinding(), "Services/CalculatorService.svc");

    builder.ConfigureAllServiceHostBase(serviceHost =>
    {
        // Add service behaviors
        var fileLogger = app.Services.GetRequiredService<IFileLogger>();
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

var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
serviceMetadataBehavior.HttpGetEnabled = true;

app.Run();
