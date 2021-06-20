using System.Text.Json.Serialization;

namespace PrimeView.Entities
{
	public class OperatingSystemInfo
	{
		public string? Platform { get; set; }
		[JsonPropertyName("distro")]
		public string? Distribution { get; set; }
		public string? Release { get; set; }
		public string? CodeName { get; set; }
		public string? Kernel { get; set; }
		[JsonPropertyName("arch")]
		public string? Architecture { get; set; }
		public string? CodePage { get; set; }
		public string? LogoFile { get; set; }
		public string? Build { get; set; }
		public string? ServicePack { get; set; }
		[JsonPropertyName("uefi")]
		public bool? IsUefi { get; set; }
	}
}
