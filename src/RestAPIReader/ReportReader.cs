using Microsoft.Extensions.Configuration;
using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrimeView.RestAPIReader
{
	public class ReportReader : IReportReader
	{
		private readonly SortedList<int, ReportSummary> summaries = new();
		private readonly Dictionary<string, Report> reportMap = new();
		private readonly Service.PrimesAPI primesAPI;
		private int totalReports = 0;

		public ReportReader(IConfiguration configuration)
		{
			this.primesAPI = new(configuration.GetValue<string>(Constants.APIBaseURI), new HttpClient());
		}

		private async Task LoadMissingSummaries(int skipFirst, int maxSummaryCount)
		{
			for (int missingIndex = skipFirst; missingIndex < skipFirst + maxSummaryCount; missingIndex++)
			{
				// find gaps in the requested key space, and fill them
				if (!this.summaries.ContainsKey(missingIndex))
				{
					int missingCount = 0;

					// count number of missing keys, but stop when we've reached the end of the key space we were asked to load
					while (!this.summaries.ContainsKey(missingIndex + ++missingCount) && (missingIndex + missingCount) < (skipFirst + maxSummaryCount));

					await LoadSummaries(missingIndex, missingCount);

					// we may not actually have been able to load all requested missing summaries, but for the sake of filling gaps
					//  for which data is available in an efficient manner, we'll act like we did; it just means some gaps may remain
					missingIndex += missingCount;
				}
			}
		}

		private async Task LoadSummaries(int skipFirst, int maxSummaryCount)
		{
			Service.SessionsResponseDto sessionsResult;
			try
			{
				sessionsResult = await this.primesAPI.GetSessionsAsync(skipFirst, maxSummaryCount);
			}
			catch (Service.ApiException e)
			{
				Console.Error.WriteLine(e);
				return;
			}

			int i = 0;
			foreach (var session in sessionsResult.Sessions)
			{
				var runner = session.Runner;

				ReportSummary summary = new()
				{
					Id = session.Id,
					Date = session.Created.DateTime,
					User = runner.Name,
					Architecture = runner.OsArch,
					CpuBrand = runner.CpuBrand,
					CpuCores = (int)runner.CpuCores,
					CpuProcessors = (int)runner.CpuProcessors,
					CpuVendor = runner.CpuVendor,
					DockerArchitecture = runner.DockerArchitecture,
					IsSystemVirtual = runner.SystemVirtual,
					OsPlatform = runner.OsPlatform,
					OsRelease = runner.OsRelease,
					ResultCount = (int)session.ResultCount
				};

				this.summaries.Add(skipFirst + i++, summary);
			}

			this.totalReports = (int)sessionsResult.Total;
		}

		public async Task<Report> GetReport(string id)
		{
			if (this.reportMap.ContainsKey(id))
				return this.reportMap[id];

			Service.Session? sessionResponse;
			try
			{
				sessionResponse = await this.primesAPI.GetSessionResultsAsync(id);
			}
			catch (Service.ApiException e)
			{
				Console.Error.WriteLine(e);
				return new Report();
			}

			var runner = sessionResponse.Runner;

			CPUInfo cpu = new()
			{
				Brand = runner.CpuBrand,
				Cores = (int)runner.CpuCores,
				EfficiencyCores = (int?)runner.CpuEfficiencyCores,
				Family = runner.CpuFamily,
				Flags = runner.CpuFlags,
				Governor = runner.CpuGovernor,
				Manufacturer = runner.CpuManufacturer,
				MaximumSpeed = (float?)runner.CpuSpeedMax,
				MinimumSpeed = (float?)runner.CpuSpeedMin,
				Model = runner.CpuModel,
				PerformanceCores = (int?)runner.CpuPerformanceCores,
				PhysicalCores = (int)runner.CpuPhysicalCores,
				Processors = (int)runner.CpuProcessors,
				RaspberryProcessor = runner.SystemRaspberryProcessor,
				Revision = runner.CpuRevision,
				Socket = runner.CpuSocket,
				Speed = (float?)runner.CpuSpeed,
				Stepping = runner.CpuStepping,
				Vendor = runner.CpuVendor,
				Virtualization = runner.CpuVirtualization,
				Voltage	= runner.CpuVoltage
			};

			Dictionary<string, object> cache = new();

			if (runner.CpuCacheL1d != null)
				cache["l1d"] = (long)runner.CpuCacheL1d;
			if (runner.CpuCacheL1i != null)
				cache["l1i"] = (long)runner.CpuCacheL1i;
			if (runner.CpuCacheL2 != null)
				cache["l2"] = (long)runner.CpuCacheL2;
			if (runner.CpuCacheL3 != null)
				cache["l3"] = (long)runner.CpuCacheL3;

			if (cache.Count > 0)
				cpu.Cache = cache;

			SystemInfo system = new()
			{
				IsVirtual = runner.SystemVirtual,
				Manufacturer = runner.SystemManufacturer,
				Model = runner.SystemModel,
				RaspberryManufacturer = runner.SystemRaspberryManufacturer,
				RaspberryRevision = runner.SystemRaspberryRevision,
				RaspberryType = runner.SystemRaspberryType,
				SKU = runner.SystemSku,
				Version = runner.SystemVersion
			};

			OperatingSystemInfo operatingSystem = new() 
			{
				Architecture = runner.OsArch,
				Build = runner.OsBuild,
				CodeName = runner.OsCodename,
				CodePage = runner.OsCodepage,
				Distribution = runner.OsDistro,
				IsUefi = runner.OsUefi,
				Kernel = runner.OsKernel,
				LogoFile = runner.OsLogofile,
				Platform = runner.OsPlatform,
				Release = runner.OsRelease,
				ServicePack = runner.OsServicepack
			};

			DockerInfo dockerInfo = new()
			{
				Architecture = runner.DockerArchitecture,
				CPUCount = (int)runner.DockerNcpu,
				KernelVersion = runner.DockerKernelVersion,
				OperatingSystem = runner.DockerOperatingSystem,
				OSType = runner.DockerOsType,
				OSVersion = runner.DockerOsVersion,
				ServerVersion = runner.DockerServerVersion,
				TotalMemory = (long)runner.DockerMemTotal
			};

			List<Result> results = new();
			foreach(var apiResult in sessionResponse.Results)
			{
				Result result = new()
				{
					Algorithm = apiResult.Algorithm,
					Duration = apiResult.Duration,
					IsFaithful = apiResult.Faithful,
					Label = apiResult.Label,
					Language = apiResult.Implementation,
					Passes = (int)apiResult.Passes,
					Solution = apiResult.Solution,
					Threads = (int)apiResult.Threads
				};

				if (int.TryParse(apiResult.Bits, out int bits))
					result.Bits = bits;

				results.Add(result);
			}

			Report report = new()
			{
				Id = sessionResponse.Id,
				Date = sessionResponse.Created.DateTime,
				User = sessionResponse.Runner.Name,
				CPU = cpu,
				System = system,
				OperatingSystem = operatingSystem,
				DockerInfo = dockerInfo,
				Results = results.ToArray()
			};

			this.reportMap[id] = report;

			return report;
		}

		public async Task<(ReportSummary[] summaries, int total)> GetSummaries(int maxSummaryCount)
		{
			return await GetSummaries(0, maxSummaryCount);
		}

		public async Task<(ReportSummary[] summaries, int total)> GetSummaries(int skipFirst, int maxSummaryCount)
		{
			await LoadMissingSummaries(skipFirst, maxSummaryCount);

			return (this.summaries.SkipWhile(pair => pair.Key < skipFirst).TakeWhile(pair => pair.Key < skipFirst + maxSummaryCount).Select(pair => pair.Value).ToArray(), this.totalReports);
		}

		public void FlushCache()
		{
			this.summaries.Clear();
			this.reportMap.Clear();
		}
	}
}
