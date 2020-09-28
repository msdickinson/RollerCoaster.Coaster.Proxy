using System.Diagnostics.CodeAnalysis;


namespace RollerCoaster.Coaster.Proxy.Models
{
    [ExcludeFromCodeCoverage]
    public class CoasterProxyOptions
    {
        public string BaseURL { get; set; }
        public int HttpClientTimeoutInSeconds { get; set; }
        public ProxyOptions Create { get; set; }
        public ProxyOptions Publish { get; set; }
        public ProxyOptions Update { get; set; }
        public ProxyOptions FetchCoasters { get; set; }
        public ProxyOptions FetchCoasterById { get; set; }
        public ProxyOptions FetchCoasterByToken { get; set; }
        public ProxyOptions Delete { get; set; }
        public ProxyOptions UserAuthorized { get; set; }
        public ProxyOptions Log { get; set; }

    }
}
