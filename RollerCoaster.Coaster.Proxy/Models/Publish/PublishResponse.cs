using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.PublishCoaster
{
    [ExcludeFromCodeCoverage]
    public class PublishResponse
    {
        public int CoasterId { get; set; }
        public string Token { get; set; }
    }
}
