using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RollerCoaster.Coaster.Proxy.Configurators;
using RollerCoaster.Coaster.Proxy.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoasterProxyService(this IServiceCollection serviceCollection, Uri baseAddress, TimeSpan httpClientTimeout)
        {
            serviceCollection.AddHttpClient<ICoasterProxyService, CoasterProxyService>(client =>
            {
                client.BaseAddress = baseAddress;
                client.Timeout = httpClientTimeout;
            });

            serviceCollection.TryAddSingleton<IConfigureOptions<CoasterProxyOptions>, CoasterProxyOptionsConfigurator>();

            return serviceCollection;
        }
    }
}
