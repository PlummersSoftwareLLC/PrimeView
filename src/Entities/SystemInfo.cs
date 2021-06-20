using System.Text.Json.Serialization;

namespace PrimeView.Entities
{
	public class SystemInfo
	{
		public string? Manufacturer { get; set; }
		public string? Model { get; set; }
		public string? Version { get; set; }
		public string? SKU { get; set; }
		[JsonPropertyName("virtual")]
		public bool? IsVirtual { get; set; }
	}
}
