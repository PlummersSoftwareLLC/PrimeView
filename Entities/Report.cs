using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeView.Entities
{
	public class Report
	{
		public string? Id { get; set; }
		public DateTime? Date { get; set; }
		public CPUInfo? CPU { get; set; }
		public OperatingSystemInfo? OperatingSystem { get; set; }
		public SystemInfo? System { get; set; }
		public DockerInfo? DockerInfo { get; set; }
		public Result[]? Results { get; set; }
	}
}
