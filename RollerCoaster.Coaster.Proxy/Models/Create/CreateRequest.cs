using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.Create
{
    [ExcludeFromCodeCoverage]
    public class CreateRequest
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
