using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.UpdateCoaster
{
    [ExcludeFromCodeCoverage]
	public class UpdateRequest
	{
		public int CoasterId { get; set; }
		public string Name { get; set; }
		public string Coaster { get; set; }
	}
}
