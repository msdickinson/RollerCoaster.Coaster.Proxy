using DickinsonBros.DurableRest.Abstractions.Models;
using RollerCoaster.Coaster.Proxy.Models.Create;
using RollerCoaster.Coaster.Proxy.Models.Delete;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterById;
using RollerCoaster.Coaster.Proxy.Models.FetchCoasterByToken;
using RollerCoaster.Coaster.Proxy.Models.PublishCoaster;
using RollerCoaster.Coaster.Proxy.Models.UpdateCoaster;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RollerCoaster.Coaster.Proxy
{
    public interface ICoasterProxyService
    {
        Task<HttpResponse<CreateResponse>> CreateAsync(CreateRequest createRequest, string bearerToken);
        Task<HttpResponse<PublishResponse>> PublishAsync(PublishRequest publishRequest, string bearerToken);
        Task<HttpResponseMessage> UpdateAsync(UpdateRequest updateCoasterRequest, string bearerToken);
        Task<HttpResponseMessage> LogAsync();
        Task<HttpResponseMessage> UserAuthorizedAsync(string bearerToken);
        Task<HttpResponse<IEnumerable<Models.FetchCoasters.Coaster>>> FetchCoastersAsync(string bearerToken);
        Task<HttpResponse<FetchCoasterByIdResponse>> FetchCoasterByIdAsync(FetchCoasterByIdRequest fetchCoasterByIdRequest, string bearerToken);
        Task<HttpResponse<FetchCoasterByTokenResponse>> FetchCoasterByTokenAsync(FetchCoasterByTokenRequest fetchCoasterByTokenRequest);
        Task<HttpResponseMessage> DeleteCoasterAsync(DeleteRequest deleteRequest, string bearerToken);
    }
}