using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.PublishCoaster
{
    [ExcludeFromCodeCoverage]
    public class PublishRequest
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
