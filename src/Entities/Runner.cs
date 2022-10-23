using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeView.Entities
{
    public class Runner
    {
        public string? Id { get; set; }
        public string? User { get; set; }
        public CPUInfo? CPU { get; set; }
        public OperatingSystemInfo? OperatingSystem { get; set; }
        public SystemInfo? System { get; set; }
        public DockerInfo? DockerInfo { get; set; }

        public string Description
        {
            get
            {
                StringBuilder builder = new();

                if (User != null)
                    builder.Append($"{User}'s ");

                if (OperatingSystem?.Architecture != null)
                    builder.Append($"{OperatingSystem.Architecture} ");

                if (CPU?.Vendor != null)
                    builder.Append($"{CPU.Vendor} ");

                if (CPU?.Brand != null)
                    builder.Append($"{CPU.Brand} ");
   
                if (CPU?.Cores != null)
                    builder.Append($"({CPU.Cores} cores) ");

                if (OperatingSystem?.Distribution != null || OperatingSystem?.Release != null)
                    builder.Append($"running ");

                if (OperatingSystem?.Distribution != null)
                    builder.Append($"{OperatingSystem.Distribution} ");

                if (OperatingSystem?.Release != null)
                    builder.Append($"{OperatingSystem.Release}");

                return builder.ToString().TrimEnd();
            }
        }
    }
}
