using DickinsonBros.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Coaster.Proxy.Configurators;
using RollerCoaster.Coaster.Proxy.Models;
using System.Threading.Tasks;


namespace RollerCoaster.Coaster.Proxy.Tests.Configurators
{
    [TestClass]
    public class CoasterProxyOptionsConfiguratorTests : BaseTest
    {
        [TestMethod]
        public async Task Configure_Runs_ConfigReturns()
        {
            var coasterProxyOptions = new CoasterProxyOptions
            {
                Create = new Models.ProxyOptions
                {
                    Resource = "SampleCreateResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                Delete = new Models.ProxyOptions
                {
                    Resource = "SampleDeleteResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                FetchCoasterById = new Models.ProxyOptions
                {
                    Resource = "SampleFetchCoasterByIdResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                FetchCoasterByToken = new Models.ProxyOptions
                {
                    Resource = "SampleFetchCoasterByTokenResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                UserAuthorized = new Models.ProxyOptions
                {
                    Resource = "SampleUserAuthorizedResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                BaseURL = "SampleBaseURL",
                HttpClientTimeoutInSeconds = 1,
                FetchCoasters = new Models.ProxyOptions
                {
                    Resource = "SampleFetchCoastersResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                Log = new Models.ProxyOptions
                {
                    Resource = "SampleLogResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                Publish = new Models.ProxyOptions
                {
                    Resource = "SamplePublishResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                },
                Update = new Models.ProxyOptions
                {
                    Resource = "SampleUpdateResource",
                    Retrys = 1,
                    TimeoutInSeconds = 2
                }
            };

            var configurationRoot = BuildConfigurationRoot(coasterProxyOptions);

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    //Act
                    var options = serviceProvider.GetRequiredService<IOptions<CoasterProxyOptions>>().Value;

                    //Assert
                    Assert.IsNotNull(options);

                    Assert.AreEqual(coasterProxyOptions.BaseURL, coasterProxyOptions.BaseURL);

                    Assert.AreEqual(coasterProxyOptions.Create.Resource                         , options.Create.Resource);
                    Assert.AreEqual(coasterProxyOptions.Create.Retrys                           , options.Create.Retrys);
                    Assert.AreEqual(coasterProxyOptions.Create.TimeoutInSeconds                 , options.Create.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.Delete.Resource                         , options.Delete.Resource);
                    Assert.AreEqual(coasterProxyOptions.Delete.Retrys                           , options.Delete.Retrys);
                    Assert.AreEqual(coasterProxyOptions.Delete.TimeoutInSeconds                 , options.Delete.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.FetchCoasterById.Resource               , options.FetchCoasterById.Resource);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasterById.Retrys                 , options.FetchCoasterById.Retrys);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasterById.TimeoutInSeconds       , options.FetchCoasterById.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.FetchCoasterByToken.Resource            , options.FetchCoasterByToken.Resource);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasterByToken.Retrys              , options.FetchCoasterByToken.Retrys);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasterByToken.TimeoutInSeconds    , options.FetchCoasterByToken.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.FetchCoasters.Resource                  , options.FetchCoasters.Resource);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasters.Retrys                    , options.FetchCoasters.Retrys);
                    Assert.AreEqual(coasterProxyOptions.FetchCoasters.TimeoutInSeconds          , options.FetchCoasters.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.HttpClientTimeoutInSeconds              , options.HttpClientTimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.Log.Resource                            , options.Log.Resource);
                    Assert.AreEqual(coasterProxyOptions.Log.Retrys                              , options.Log.Retrys);
                    Assert.AreEqual(coasterProxyOptions.Log.TimeoutInSeconds                    , options.Log.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.Publish.Resource                        , options.Publish.Resource);
                    Assert.AreEqual(coasterProxyOptions.Publish.Retrys                          , options.Publish.Retrys);
                    Assert.AreEqual(coasterProxyOptions.Publish.TimeoutInSeconds                , options.Publish.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.Update.Resource                         , options.Update.Resource);
                    Assert.AreEqual(coasterProxyOptions.Update.Retrys                           , options.Update.Retrys);
                    Assert.AreEqual(coasterProxyOptions.Update.TimeoutInSeconds                 , options.Update.TimeoutInSeconds);

                    Assert.AreEqual(coasterProxyOptions.UserAuthorized.Resource                 , options.UserAuthorized.Resource);
                    Assert.AreEqual(coasterProxyOptions.UserAuthorized.Retrys                   , options.UserAuthorized.Retrys);
                    Assert.AreEqual(coasterProxyOptions.UserAuthorized.TimeoutInSeconds         , options.UserAuthorized.TimeoutInSeconds);

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection, configurationRoot)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<IConfigureOptions<CoasterProxyOptions>, CoasterProxyOptionsConfigurator>();

            return serviceCollection;
        }

        #endregion
    }
}
