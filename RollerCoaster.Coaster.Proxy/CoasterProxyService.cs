using DickinsonBros.DurableRest.Abstractions;
using DickinsonBros.DurableRest.Abstractions.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using RollerCoaster.Coaster.Proxy.Models;
using RollerCoaster.Coaster.Proxy.Models.Create;
using RollerCoaster.Coaster.Proxy.Models.Delete;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterById;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterByToken;
using RollerCoaster.Coaster.Proxy.Models.PublishCoaster;
using RollerCoaster.Coaster.Proxy.Models.UpdateCoaster;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace RollerCoaster.Coaster.Proxy
{
    public class CoasterProxyService : ICoasterProxyService
    {
        internal readonly CoasterProxyOptions _coasterProxyOptions;
        internal readonly IDurableRestService _durableRestService;
        internal readonly HttpClient _httpClient;

        public const string AUTHORIZATION = "Authorization";

        public CoasterProxyService(IDurableRestService durableRestService, HttpClient httpClient, IOptions<CoasterProxyOptions> coasterProxyOptions)
        {
            _durableRestService = durableRestService;
            _coasterProxyOptions = coasterProxyOptions.Value;
            _httpClient = httpClient;
        }

        public async Task<HttpResponse<CreateResponse>> CreateAsync(CreateRequest createRequest, string bearerToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_coasterProxyOptions.Create.Resource, UriKind.Relative),
                Content = new StringContent(JsonSerializer.Serialize(createRequest), Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync<CreateResponse>(_httpClient, httpRequestMessage, _coasterProxyOptions.Create.Retrys, _coasterProxyOptions.Create.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponse<PublishResponse>> PublishAsync(PublishRequest publishRequest, string bearerToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_coasterProxyOptions.Publish.Resource, UriKind.Relative),
                Content = new StringContent(JsonSerializer.Serialize(publishRequest), Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync<PublishResponse>(_httpClient, httpRequestMessage, _coasterProxyOptions.Publish.Retrys, _coasterProxyOptions.Publish.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateRequest updateCoasterRequest, string bearerToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(_coasterProxyOptions.Update.Resource, UriKind.Relative),
                Content = new StringContent(JsonSerializer.Serialize(updateCoasterRequest), Encoding.UTF8, "application/json")
            };

            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync(_httpClient, httpRequestMessage, _coasterProxyOptions.Update.Retrys, _coasterProxyOptions.Update.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponse<IEnumerable<Models.FetchCoasters.Coaster>>> FetchCoastersAsync(string bearerToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_coasterProxyOptions.FetchCoasters.Resource, UriKind.Relative),
            };
            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync<IEnumerable<Models.FetchCoasters.Coaster>>(_httpClient, httpRequestMessage, _coasterProxyOptions.FetchCoasters.Retrys, _coasterProxyOptions.FetchCoasters.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponse<FetchCoasterByIdResponse>>FetchCoasterByIdAsync(FetchCoasterByIdRequest fetchCoasterByIdRequest, string bearerToken)
        {
            var resourceWithPrams = QueryHelpers.AddQueryString(_coasterProxyOptions.FetchCoasterById.Resource, nameof(fetchCoasterByIdRequest.CoasterId), fetchCoasterByIdRequest.CoasterId.ToString());

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(resourceWithPrams, UriKind.Relative),
            };
            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);
          
            return await _durableRestService.ExecuteAsync<FetchCoasterByIdResponse>(_httpClient, httpRequestMessage, _coasterProxyOptions.FetchCoasterById.Retrys, _coasterProxyOptions.FetchCoasterById.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponse<FetchCoasterByTokenResponse>> FetchCoasterByTokenAsync(FetchCoasterByTokenRequest fetchCoasterByTokenRequest)
        {
            var resourceWithPrams = QueryHelpers.AddQueryString(_coasterProxyOptions.FetchCoasterByToken.Resource, nameof(fetchCoasterByTokenRequest.Token), fetchCoasterByTokenRequest.Token);

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(resourceWithPrams, UriKind.Relative),
            };

            return await _durableRestService.ExecuteAsync<FetchCoasterByTokenResponse>(_httpClient, httpRequestMessage, _coasterProxyOptions.FetchCoasterByToken.Retrys, _coasterProxyOptions.FetchCoasterByToken.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> DeleteCoasterAsync(DeleteRequest deleteRequest, string bearerToken)
        {
            var resourceWithPrams = QueryHelpers.AddQueryString(_coasterProxyOptions.Delete.Resource, nameof(deleteRequest.CoasterId), deleteRequest.CoasterId.ToString());

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(resourceWithPrams, UriKind.Relative),
            };

            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync(_httpClient, httpRequestMessage, _coasterProxyOptions.Delete.Retrys, _coasterProxyOptions.Delete.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> LogAsync()
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_coasterProxyOptions.Log.Resource, UriKind.Relative),
            };

            return await _durableRestService.ExecuteAsync(_httpClient, httpRequestMessage, _coasterProxyOptions.Log.Retrys, _coasterProxyOptions.Log.TimeoutInSeconds).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> UserAuthorizedAsync(string bearerToken)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_coasterProxyOptions.UserAuthorized.Resource, UriKind.Relative)
            };

            httpRequestMessage.Headers.Add(AUTHORIZATION, bearerToken);

            return await _durableRestService.ExecuteAsync(_httpClient, httpRequestMessage, _coasterProxyOptions.UserAuthorized.Retrys, _coasterProxyOptions.UserAuthorized.TimeoutInSeconds).ConfigureAwait(false);
        }
    }
}
