using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models
{
    [ExcludeFromCodeCoverage]
    public class ProxyOptions
    {
        public double TimeoutInSeconds { get; set; }
        public int Retrys { get; set; }
        public string Resource { get; set; }
    }
}
