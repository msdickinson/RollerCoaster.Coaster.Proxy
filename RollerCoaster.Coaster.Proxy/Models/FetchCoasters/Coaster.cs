using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.FetchCoasters
{
    [ExcludeFromCodeCoverage]
	public class Coaster
	{
		public int CoasterId { get; set; }
		public string Name { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }
		public bool Published { get; set; }
		public string PublishToken { get; set; }
	}
}
