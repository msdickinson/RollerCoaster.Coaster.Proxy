using System;
using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Coaster.Proxy.Models.FetchCoasterByToken
{
	[ExcludeFromCodeCoverage]
	public class FetchCoasterByTokenResponse
	{
		public int CoasterId { get; set; }
		public string Name { get; set; }
		public int AccountId { get; set; }
		public string Username { get; set; }
		public string Token { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateUpdated { get; set; }
		public string Data { get; set; }
	}
}
