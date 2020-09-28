using DickinsonBros.DurableRest.Abstractions;
using DickinsonBros.DurableRest.Abstractions.Models;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Coaster.Proxy.Models;
using RollerCoaster.Coaster.Proxy.Models.Create;
using RollerCoaster.Coaster.Proxy.Models.Delete;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterById;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterByToken;
using RollerCoaster.Coaster.Proxy.Models.PublishCoaster;
using RollerCoaster.Coaster.Proxy.Models.UpdateCoaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RollerCoaster.Coaster.Proxy.Tests
{
  [TestClass]
    public class CoasterProxyServiceTests : BaseTest
    {
        const string BASE_URL = "https://localhost8080";
        const int HTTP_CLIENT_TIMEOUT_IN_SECONDS = 30;

        //Create ProxyOptions
        const string CREATE_PROXY_OPTION_RESOURCE = "SampleCreateResource";
        const int CREATE_PROXY_OPTION_RETRYS = 1;
        const double CREATE_PROXY_OPTION_TIMEOUT_IN_SECONDS = 1;

        //Publish ProxyOptions
        const string PUBLISH_PROXY_OPTION_RESOURCE = "SamplePublishResource";
        const int PUBLISH_PROXY_OPTION_RETRYS = 2;
        const double PUBLISH_PROXY_OPTION_TIMEOUT_IN_SECONDS = 2;

        //Update Proxy Options
        const string UPDATE_PROXY_OPTION_RESOURCE = "SampleUpdateResource";
        const int UPDATE_PROXY_OPTION_RETRYS = 3;
        const double UPDATE_PROXY_OPTION_TIMEOUT_IN_SECONDS = 3;

        //FetchCoasters ProxyOptions
        const string FETCH_COASTERS_PROXY_OPTION_RESOURCE = "SampleFetchCoastersResource";
        const int FETCH_COASTERS_PROXY_OPTION_RETRYS = 4;
        const double FETCH_COASTERS_PROXY_OPTION_TIMEOUT_IN_SECONDS = 4;

        //FetchCoasterById ProxyOptions
        const string FETCH_COASTER_BY_ID_PROXY_OPTION_RESOURCE = "SampleFetchCoasterByIdResource";
        const int FETCH_COASTER_BY_ID_PROXY_OPTION_RETRYS = 5;
        const double FETCH_COASTER_BY_ID_PROXY_OPTION_TIMEOUT_IN_SECONDS = 5;

        //FetchCoasterByToken ProxyOptions
        const string FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RESOURCE = "SampleFetchCoasterByTokenResource";
        const int FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RETRYS = 6;
        const double FETCH_COASTER_BY_TOKEN_PROXY_OPTION_TIMEOUT_IN_SECONDS = 6;

        //Delete ProxyOptions
        const string DELETE_PROXY_OPTION_RESOURCE = "SampleDeleteResource";
        const int DELETE_PROXY_OPTION_RETRYS = 7;
        const double DELETE_PROXY_OPTION_TIMEOUT_IN_SECONDS = 7;

        //UserAuthorizedOptions
        const string USER_AUTHORIZED_PROXY_OPTION_RESOURCE = "SampleUserAuthorizedResource";
        const int USER_AUTHORIZED_PROXY_OPTION_RETRYS = 5;
        const double USER_AUTHORIZED_PROXY_OPTION_TIMEOUT_IN_SECONDS = 5;

        //LogProxyOptions
        const string LOG_PROXY_OPTION_RESOURCE = "SampleLogResource";
        const int LOG_PROXY_OPTION_RETRYS = 3;
        const double LOG_PROXY_OPTION_TIMEOUT_IN_SECONDS = 3;

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var createRequest = new CreateRequest();
                    var httpResponse = new HttpResponse<CreateResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{CREATE_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<CreateResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.CreateAsync(createRequest, bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync<CreateResponse>
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.AreEqual(expectedContent.ToString(), observedHttpRequestMessage.Content.ToString());
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Create.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Create.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task CreateAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var createRequest = new CreateRequest();
                    var httpResponse = new HttpResponse<CreateResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{CREATE_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<CreateResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.CreateAsync(createRequest, bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region PublishAsync

        [TestMethod]
        public async Task PublishAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var publishRequest = new PublishRequest();
                    var httpResponse = new HttpResponse<PublishResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{PUBLISH_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(publishRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<PublishResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.PublishAsync(publishRequest, bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync<PublishResponse>
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.AreEqual(expectedContent.ToString(), observedHttpRequestMessage.Content.ToString());
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Publish.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Publish.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task PublishAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var publishRequest = new PublishRequest();
                    var httpResponse = new HttpResponse<PublishResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{PUBLISH_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(publishRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<PublishResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.PublishAsync(publishRequest, bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UpdateAsync

        [TestMethod]
        public async Task UpdateAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var updateRequest = new UpdateRequest();
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Put;
                    var expectedRequestUri = new Uri($"{UPDATE_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.UpdateAsync(updateRequest, bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.AreEqual(expectedContent.ToString(), observedHttpRequestMessage.Content.ToString());
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Update.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Update.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UpdateAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var updateRequest = new UpdateRequest();
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Put;
                    var expectedRequestUri = new Uri($"{UPDATE_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.UpdateAsync(updateRequest, bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region LogAsync

        [TestMethod]
        public async Task LogAsync_Runs_durableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{LOG_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.LogAsync();

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Log.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Log.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task LogAsync_Runs_ReturnsHttpResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{LOG_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.LogAsync();

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UserAuthorizedAsync

        [TestMethod]
        public async Task UserAuthorizedAsync_Runs_durableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{USER_AUTHORIZED_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.UserAuthorizedAsync(bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.UserAuthorized.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.UserAuthorized.TimeoutInSeconds, (double)observedTimeoutInSeconds);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UserAuthorizedAsync_Runs_ReturnsHttpResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Post;
                    var expectedRequestUri = new Uri($"{USER_AUTHORIZED_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.UserAuthorizedAsync(bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchCoastersAsync

        [TestMethod]
        public async Task FetchCoastersAsync_Runs_durableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var httpResponse = new HttpResponse<IEnumerable<Models.FetchCoasters.Coaster>>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTERS_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<IEnumerable<Models.FetchCoasters.Coaster>>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoastersAsync(bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync<IEnumerable<Models.FetchCoasters.Coaster>>
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString, observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasters.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasters.TimeoutInSeconds, (double)observedTimeoutInSeconds);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task FetchCoastersAsync_Runs_ReturnsHttpResponseMessage()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var httpResponse = new HttpResponse<IEnumerable<Models.FetchCoasters.Coaster>>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTERS_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<IEnumerable<Models.FetchCoasters.Coaster>>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoastersAsync(bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);

                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchCoasterByIdAsync

        [TestMethod]
        public async Task FetchCoasterByIdAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var fetchCoasterByIdRequest = new FetchCoasterByIdRequest
                    {
                        CoasterId = 1
                    };
                    var httpResponse = new HttpResponse<FetchCoasterByIdResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTER_BY_ID_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(fetchCoasterByIdRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<FetchCoasterByIdResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoasterByIdAsync(fetchCoasterByIdRequest, bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync<FetchCoasterByIdResponse>
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString + $"?CoasterId={fetchCoasterByIdRequest.CoasterId}", observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasterById.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasterById.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task FetchCoasterByIdAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var fetchCoasterByIdRequest = new FetchCoasterByIdRequest
                    {
                        CoasterId = 1
                    };
                    var httpResponse = new HttpResponse<FetchCoasterByIdResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTER_BY_ID_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(fetchCoasterByIdRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<FetchCoasterByIdResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoasterByIdAsync(fetchCoasterByIdRequest, bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region FetchCoasterByTokenAsync

        [TestMethod]
        public async Task FetchCoasterByTokenAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var fetchCoasterByTokenRequest = new FetchCoasterByTokenRequest { Token = "SampleToken" };
                    var httpResponse = new HttpResponse<FetchCoasterByTokenResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(fetchCoasterByTokenRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<FetchCoasterByTokenResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoasterByTokenAsync(fetchCoasterByTokenRequest);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync<FetchCoasterByTokenResponse>
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.AreEqual(expectedRequestUri.OriginalString + $"?Token={fetchCoasterByTokenRequest.Token}", observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasterByToken.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.FetchCoasterByToken.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task FetchCoasterByTokenAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var fetchCoasterByTokenRequest = new FetchCoasterByTokenRequest { Token = "SampleToken" };
                    var httpResponse = new HttpResponse<FetchCoasterByTokenResponse>();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Get;
                    var expectedRequestUri = new Uri($"{FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RESOURCE}", UriKind.Relative);
                    var expectedContent = new StringContent(JsonSerializer.Serialize(fetchCoasterByTokenRequest), Encoding.UTF8, "application/json");


                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync<FetchCoasterByTokenResponse>
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.FetchCoasterByTokenAsync(fetchCoasterByTokenRequest);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region DeleteCoasterAsync

        [TestMethod]
        public async Task DeleteCoasterAsync_Runs_DurableRestServiceExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var deleteRequest = new DeleteRequest { CoasterId = 1 };
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Delete;
                    var expectedRequestUri = new Uri($"{DELETE_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.DeleteCoasterAsync(deleteRequest, bearerToken);

                    //Assert
                    durableRestServiceMock
                    .Verify(
                        durableRestService => durableRestService.ExecuteAsync
                        (
                            It.IsAny<HttpClient>(),
                            It.IsAny<HttpRequestMessage>(),
                            It.IsAny<int>(),
                            It.IsAny<double>()
                        ),
                        Times.Once
                    );

                    Assert.AreEqual(uutConcrete._httpClient, observedHttpClient);
                    Assert.AreEqual(expectedMethod, observedHttpRequestMessage.Method);
                    Assert.IsTrue(observedHttpRequestMessage.Headers.First(e => e.Key == CoasterProxyService.AUTHORIZATION).Value.First() == bearerToken);
                    Assert.AreEqual(expectedRequestUri.OriginalString + $"?CoasterId={deleteRequest.CoasterId}", observedHttpRequestMessage.RequestUri.OriginalString);
                    Assert.IsNull(observedHttpRequestMessage.Content);
                    Assert.IsNotNull(observedRetrys);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Delete.Retrys, (int)observedRetrys);
                    Assert.IsNotNull(observedTimeoutInSeconds);
                    Assert.AreEqual(uutConcrete._coasterProxyOptions.Delete.TimeoutInSeconds, (double)observedTimeoutInSeconds);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task DeleteCoasterAsync_Runs_ReturnsHttpResponse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var bearerToken = "SampleBearerToken";
                    var deleteRequest = new DeleteRequest { CoasterId = 1 };
                    var httpResponse = new HttpResponseMessage();

                    var observedHttpClient = (HttpClient)null;
                    var observedHttpRequestMessage = (HttpRequestMessage)null;
                    var observedRetrys = (int?)null;
                    var observedTimeoutInSeconds = (double?)null;

                    var expectedMethod = HttpMethod.Delete;
                    var expectedRequestUri = new Uri($"{DELETE_PROXY_OPTION_RESOURCE}", UriKind.Relative);

                    var durableRestServiceMock = serviceProvider.GetMock<IDurableRestService>();
                    durableRestServiceMock
                        .Setup
                        (
                            durableRestService => durableRestService.ExecuteAsync
                            (
                                It.IsAny<HttpClient>(),
                                It.IsAny<HttpRequestMessage>(),
                                It.IsAny<int>(),
                                It.IsAny<double>()
                            )
                        )
                        .Callback((HttpClient httpClient, HttpRequestMessage httpRequestMessage, int retrys, double timeoutInSeconds) =>
                        {
                            observedHttpClient = httpClient;
                            observedHttpRequestMessage = httpRequestMessage;
                            observedRetrys = retrys;
                            observedTimeoutInSeconds = timeoutInSeconds;
                        })
                        .ReturnsAsync(httpResponse);

                    var uut = serviceProvider.GetRequiredService<ICoasterProxyService>();
                    var uutConcrete = (CoasterProxyService)uut;

                    //Act
                    var observed = await uut.DeleteCoasterAsync(deleteRequest, bearerToken);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(httpResponse, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICoasterProxyService, CoasterProxyService>();
            serviceCollection.AddSingleton(Mock.Of<IDurableRestService>());
            serviceCollection.AddSingleton(Mock.Of<HttpClient>());
            serviceCollection.AddOptions<CoasterProxyOptions>()
                .Configure((accountProxyOptions) =>
                {

                    accountProxyOptions.BaseURL = BASE_URL;
                    accountProxyOptions.HttpClientTimeoutInSeconds = HTTP_CLIENT_TIMEOUT_IN_SECONDS;
                    accountProxyOptions.Create = new ProxyOptions
                    {
                        Resource = CREATE_PROXY_OPTION_RESOURCE,
                        Retrys = CREATE_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = CREATE_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.Publish = new ProxyOptions
                    {
                        Resource = PUBLISH_PROXY_OPTION_RESOURCE,
                        Retrys = PUBLISH_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = PUBLISH_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.Update = new ProxyOptions
                    {
                        Resource = UPDATE_PROXY_OPTION_RESOURCE,
                        Retrys = UPDATE_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = UPDATE_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.FetchCoasters = new ProxyOptions
                    {
                        Resource = FETCH_COASTERS_PROXY_OPTION_RESOURCE,
                        Retrys = FETCH_COASTERS_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = FETCH_COASTERS_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.FetchCoasterById = new ProxyOptions
                    {
                        Resource = FETCH_COASTER_BY_ID_PROXY_OPTION_RESOURCE,
                        Retrys = FETCH_COASTER_BY_ID_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = FETCH_COASTER_BY_ID_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.FetchCoasterByToken = new ProxyOptions
                    {
                        Resource = FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RESOURCE,
                        Retrys = FETCH_COASTER_BY_TOKEN_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = FETCH_COASTER_BY_TOKEN_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.Delete = new ProxyOptions
                    {
                        Resource = DELETE_PROXY_OPTION_RESOURCE,
                        Retrys = DELETE_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = DELETE_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.UserAuthorized = new ProxyOptions
                    {
                        Resource = USER_AUTHORIZED_PROXY_OPTION_RESOURCE,
                        Retrys = USER_AUTHORIZED_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = USER_AUTHORIZED_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                    accountProxyOptions.Log = new ProxyOptions
                    {
                        Resource = LOG_PROXY_OPTION_RESOURCE,
                        Retrys = LOG_PROXY_OPTION_RETRYS,
                        TimeoutInSeconds = LOG_PROXY_OPTION_TIMEOUT_IN_SECONDS
                    };
                });


            return serviceCollection;
        }
        #endregion
    }
}
