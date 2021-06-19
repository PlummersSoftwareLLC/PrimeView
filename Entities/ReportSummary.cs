using System;

namespace PrimeView.Entities
{
	public class ReportSummary
	{
		public string? Id { get; set; }
		public DateTime? Date { get; set; }
		public string? CpuVendor { get; set; }
		public string? CpuBrand { get; set; }
		public int? CpuCores { get; set; }
		public int? CpuProcessors { get; set; }
		public string? OsPlatform { get; set; }
		public string? OsRelease { get; set; }
		public string? Architecture { get; set; }
		public bool? IsSystemVirtual { get; set; }
		public string? DockerArchitecture { get; set; }
		public int ResultCount { get; set; } = 0;
	}
}
