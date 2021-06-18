using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
		public string? CPUCount { get; set; }
		[JsonPropertyName("memTotal")]
		public string? TotalMemory { get; set; }
		public string? ServerVersion { get; set; }
	}
}
