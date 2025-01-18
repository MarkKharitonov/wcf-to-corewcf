// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using WCF.SampleService.Behaviors.ErrorBehavior;
using ErrorBehaviorNS = WCF.SampleService.Behaviors.ErrorBehavior;
using CoreWCF.SampleService.Test;
using CoreWCF.Configuration;
using CoreWCF;
using SM = CoreWCF;
using SSM = System.ServiceModel;
using WCF.SampleService.Loggers;
using WCF.SampleService.Behaviors.HeaderValidationBehavior;

namespace WCF.SampleService.Test.Behaviors.ErrorBehavior
{
    [TestFixture]
    public class CoreWCFHeaderValidationBehaviorTests
    {
        static CoreWCFHostingEnvironment hostingEnv;
        static Mock<IFileLogger> fileLogger;
        static string serviceUrl;
        static string baseUrl = "http://127.0.0.1:40000";

        [SetUp]
        public void Initialize()
        {
            serviceUrl = $"{baseUrl}/TestService.svc";
            fileLogger = new Mock<IFileLogger>();
            hostingEnv = CoreWCFHostingEnvironment.Run<CoreWCFStartup>(baseUrl);
        }

        [TearDown]
        public void CleanUp()
        {
            hostingEnv.Dispose();
        }

        static ITestService Channel => hostingEnv.GetChannel<ITestService>(serviceUrl);

        class CoreWCFStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddServiceModelServices();
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseServiceModel(builder =>
                {
                    builder.AddService<TestService>(new[] { new ErrorBehaviorNS.ErrorBehavior(typeof(GlobalErrorHandler), fileLogger.Object) });
                    builder.AddEndpoint<TestService, ITestService>(new[] { new HeaderValidationBehavior() }, new BasicHttpBinding(), serviceUrl, null);
                });
            }
        }

        #region tests

        [Test]
        public void HeadersAreValidatedByHeaderValidationBehavior()
        {
            fileLogger.Setup(t => t.Log(It.IsAny<string>()));

            var expectedException = Assert.Throws<SSM.FaultException>(() => Channel.Test(2, 1));

            Assert.IsNotNull(expectedException);
            Assert.AreEqual("ClientId was not found the request.", expectedException.Message);
            fileLogger.Verify(t => t.Log(It.IsAny<string>()), Times.Once);
        }
        #endregion

        #region service
        [SM.ServiceContract]
        [SSM.ServiceContract]
        interface ITestService
        {
            [SM.OperationContract]
            [SSM.OperationContract]
            int Test(int num1, int num2);
        }

        class TestService : ITestService
        {
            public int Test(int num1, int num2)
            {
                return num1 / num2;
            }
        }
        #endregion

    }
}
