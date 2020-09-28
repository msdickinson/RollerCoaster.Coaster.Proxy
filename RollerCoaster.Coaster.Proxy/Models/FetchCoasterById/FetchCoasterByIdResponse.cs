using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.FetchCoasterById
{
    [ExcludeFromCodeCoverage]
    public class FetchCoasterByIdResponse
    {
        public int CoasterId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Data { get; set; }
    }
}
