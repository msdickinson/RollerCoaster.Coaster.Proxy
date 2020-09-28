using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RollerCoaster.Coaster.Proxy.Models;

namespace RollerCoaster.Coaster.Proxy.Configurators
{
    public class CoasterProxyOptionsConfigurator : IConfigureOptions<CoasterProxyOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public CoasterProxyOptionsConfigurator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        void IConfigureOptions<CoasterProxyOptions>.Configure(CoasterProxyOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var configuration = provider.GetRequiredService<IConfiguration>();
                configuration.Bind($"{nameof(CoasterProxyOptions)}", options);
            }
        }
    }
}
