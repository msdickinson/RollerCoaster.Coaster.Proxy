using DickinsonBros.DateTime.Extensions;
using DickinsonBros.DurableRest.Extensions;
using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Encryption.Certificate.Models;
using DickinsonBros.Guid.Abstractions;
using DickinsonBros.Guid.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Redactor.Models;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Abstractions;
using DickinsonBros.Telemetry.Extensions;
using DickinsonBros.Telemetry.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RollerCoaster.Coaster.Proxy.Extensions;
using RollerCoaster.Coaster.Proxy.Models;
using RollerCoaster.Coaster.Proxy.Runner.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RollerCoaster.Coaster.Proxy.Runner
{
    class Program
    {
        IConfiguration _configuration;
        async static Task Main()
        {
            await new Program().DoMain();
        }
        async Task DoMain()
        {
            try
            {
                var services = InitializeDependencyInjection();
                ConfigureServices(services);
                using var provider = services.BuildServiceProvider();
                var telemetryService = provider.GetRequiredService<ITelemetryService>();
                var guidService = provider.GetRequiredService<IGuidService>();
                var coasterProxyService = provider.GetRequiredService<ICoasterProxyService>();
                var hostApplicationLifetime = provider.GetService<IHostApplicationLifetime>();

                var restResponse = await coasterProxyService.LogAsync();

                await telemetryService.FlushAsync().ConfigureAwait(false);

                hostApplicationLifetime.StopApplication();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("End...");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging(cfg => cfg.AddConsole());

            //Add ApplicationLifetime
            services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            //Add Stack
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddGuidService();
            services.AddLoggingService();
            services.AddRedactorService();
            services.AddConfigurationEncryptionService();
            services.AddTelemetryService();
            services.AddDurableRestService();

            //Add Account Coaster Service
            services.AddCoasterProxyService
            (
                new Uri(_configuration[$"{nameof(CoasterProxyOptions)}:{nameof(CoasterProxyOptions.BaseURL)}"]),
                new TimeSpan(0, 0, Convert.ToInt32(_configuration[$"{nameof(CoasterProxyOptions)}:{nameof(CoasterProxyOptions.HttpClientTimeoutInSeconds)}"]))
            );
        }

        IServiceCollection InitializeDependencyInjection()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("BUILD_CONFIGURATION");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true);
            _configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddSingleton(_configuration);
            return services;
        }
    }
}
