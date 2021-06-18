using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PrimeView.Entities
{
	public class CPUInfo
	{
		public string? Manufacturer { get; set; }
		public string? Brand { get; set; }
		public string? Vendor { get; set; }
		public string? Family { get; set; }
		public string? Model { get; set; }
		public string? Stepping { get; set; }
		public string? Revision { get; set; }
		public string? Voltage { get; set; }
		public float? Speed { get; set; }
		[JsonPropertyName("speedMin")]
		public float? MinimumSpeed { get; set; }
		[JsonPropertyName("speedMax")]
		public float? MaximumSpeed { get; set; }
		public string? Governor { get; set; }
		public int? Cores { get; set; }
		public int? PhysicalCores { get; set; }
		public int? Processors { get; set; }
		public string? Socket { get; set; }
		public string? Flags { get; set; }
		public string[]? FlagValues => Flags?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		public bool? Virtualization { get; set; }
		public Dictionary<string, int>? Cache { get; set; }
	}
}
