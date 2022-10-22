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
        public Result[]? Results { get; set; }

        public string Desciption
        {
            get
            {
                StringBuilder builder = new();

                if (User != null)
                    builder.Append($"{User}'s ");

                if (OperatingSystem?.Architecture != null)
                    builder.Append($"{OperatingSystem.Architecture} ");

                if (CPU?.Manufacturer != null)
                    builder.Append($"{CPU.Manufacturer} ");

                if (CPU?.Brand != null)
                    builder.Append($"{CPU.Brand} ");
   
                if (CPU?.Cores != null)
                    builder.Append($"({CPU.Cores} cores) ");

                if (OperatingSystem?.Distribution != null)
                    builder.Append($"running {OperatingSystem.Distribution} ");

                if (OperatingSystem?.Release != null)
                    builder.Append($"{OperatingSystem.Release}");

                return builder.ToString().TrimEnd();
            }
        }
    }
}
