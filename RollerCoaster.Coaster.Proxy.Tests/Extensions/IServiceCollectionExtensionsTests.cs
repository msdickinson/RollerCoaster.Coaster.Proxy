using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using RollerCoaster.Coaster.Proxy.Extensions;
using RollerCoaster.Coaster.Proxy.Models;
using RollerCoaster.Coaster.Proxy.Configurators;

namespace RollerCoaster.Coaster.Proxy.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddDateTimeService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var baseAddress = new Uri("https://Localhost:8080");
            var httpClientTimeout = new TimeSpan(0, 0, 30);

            // Act
            serviceCollection.AddCoasterProxyService(baseAddress, httpClientTimeout);


            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ICoasterProxyService) &&
                                                       serviceDefinition.ImplementationFactory != null &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Transient));

            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IConfigureOptions<CoasterProxyOptions>) &&
                               serviceDefinition.ImplementationType == typeof(CoasterProxyOptionsConfigurator) &&
                               serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
