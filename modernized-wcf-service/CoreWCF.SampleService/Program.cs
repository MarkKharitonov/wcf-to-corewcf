// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.SampleService;
using CoreWCF.SampleService.BL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using WCF.SampleService.Behaviors.ErrorBehavior;
using WCF.SampleService.Behaviors.HeaderValidationBehavior;
using WCF.SampleService.Contracts;
using WCF.SampleService.Loggers;
using WCF.SampleService.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var hostBuilder = Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        var host = hostBuilder.Build();

        host.Run();
    }
}