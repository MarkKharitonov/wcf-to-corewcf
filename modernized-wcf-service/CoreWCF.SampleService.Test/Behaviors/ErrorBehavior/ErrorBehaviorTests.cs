// Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: MIT-0

#if NETFRAMEWORK
using NUnit.Framework;
using Moq;
using System;
using WCF.SampleService.Behaviors.ErrorBehavior;
using WCF.SampleService.Loggers;
using ErrorBehaviorNS = WCF.SampleService.Behaviors.ErrorBehavior;
using System.ServiceModel;

namespace WCF.SampleService.Test.Behaviors.ErrorBehavior
{
    [TestFixture]
    public class ErrorBehaviorTests
    {
        static TestServiceHostingEnvironment<TestService> hostingEnv;
        static Mock<IFileLogger> fileLogger;
        static string serviceUrl;

        [SetUp]
        public void Initialize()
        {
            serviceUrl = "http://localhost:40000/TestService.svc";
            fileLogger = new Mock<IFileLogger>();
            hostingEnv = new TestServiceHostingEnvironment<TestService>(serviceUrl, new ErrorBehaviorNS.ErrorBehavior(typeof(GlobalErrorHandler), fileLogger.Object));
        }

        [TearDown]
        public void CleanUp()
        {
            hostingEnv.Dispose();
        }

        static ITestService Channel => hostingEnv.GetChannel<ITestService>(serviceUrl);

        [Test]
        public void SuccessfulCall()
        {
            int result = Channel.Test(2, 1);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void ExceptionIsHandledByErrorBehavior()
        {
            fileLogger.Setup(t => t.Log(It.IsAny<string>()));

            Assert.Throws<FaultException>(() => Channel.Test(2, 0));

            fileLogger.Verify(t => t.Log(It.IsAny<string>()), Times.Once);
        }

        #region service

        [ServiceContract]
        interface ITestService
        {
            [OperationContract]
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
#endif
