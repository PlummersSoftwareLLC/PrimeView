using System.Text.Json.Serialization;

namespace PrimeView.Entities
{
	public class DockerInfo
	{
		public string? KernelVersion { get; set; }
		public string? OperatingSystem { get; set; }
		public string? OSVersion { get; set; }
		public string? OSType { get; set; }
		public string? Architecture { get; set; }
		[JsonPropertyName("ncpu")]
		public int? CPUCount { get; set; }
		[JsonPropertyName("memTotal")]
		public long? TotalMemory { get; set; }
		public string? ServerVersion { get; set; }
	}
}
