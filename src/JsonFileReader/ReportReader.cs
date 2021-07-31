using Microsoft.Extensions.Configuration;
using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeView.JsonFileReader
{
	public class ReportReader : IReportReader
	{
		private List<ReportSummary>? summaries;
		private Dictionary<string, Report>? reportMap;
		private readonly HttpClient httpClient;
		private readonly string? indexFileName;
		private bool haveJsonFilesLoaded = false;

		public ReportReader(string baseAddress, IConfiguration configuration)
		{
			this.httpClient = new HttpClient { BaseAddress = new Uri(configuration.GetValue<string?>(Constants.BaseURI, baseAddress)!) };
			this.indexFileName = configuration.GetValue<string?>(Constants.Index, null);
		}

		private async Task LoadReportJsonFiles()
		{
			if (this.haveJsonFilesLoaded)
			{
				return;
			}

			string[]? reportFileNames = null;

			if (this.indexFileName != null)
			{
				try
				{
					reportFileNames = JsonSerializer.Deserialize<string[]>(await this.httpClient.GetStringAsync(this.indexFileName));
				}
				catch {}
			}

			this.summaries = new();
			this.reportMap = new();

			for (int fileIndex = 0; fileIndex != reportFileNames?.Length; fileIndex++)
			{
				string fileName = reportFileNames != null ? reportFileNames[fileIndex] : $"data/report{fileIndex + 1}.json";
				string reportJson;

				try
				{
  				reportJson = await this.httpClient.GetStringAsync(fileName);
				}
				catch (HttpRequestException)
				{
					// break out of the loop on error if we're not reading a list of files we retrieved from the index file...
					if (reportFileNames == null)
						break;

					// ...otherwise try reading the next file
					else
						continue;
				}

				var reportElement = JsonDocument.Parse(reportJson).RootElement;
				Report report = ParseReportElement(reportJson, reportElement, fileName);

				this.reportMap[report.Id!] = report;
				this.summaries.Add(ExtractSummary(report));
			}

			this.haveJsonFilesLoaded = true;
		}

		private static ReportSummary ExtractSummary(Report report)
		{
			return new()
			{
				Id = report.Id,
				Architecture = report.OperatingSystem?.Architecture,
				CpuBrand = report.CPU?.Brand,
				CpuCores = report.CPU?.Cores,
				CpuProcessors = report.CPU?.Processors,
				CpuVendor = report.CPU?.Vendor,
				Date = report.Date,
				DockerArchitecture = report.DockerInfo?.Architecture,
				IsSystemVirtual = report.System?.IsVirtual,
				OsPlatform = report.OperatingSystem?.Platform,
				OsRelease = report.OperatingSystem?.Release,
				ResultCount = report.Results?.Length ?? 0,
				User = report.User
			};
		}

		private static Report ParseReportElement(string json, JsonElement element, string? id = null)
		{
			var machineElement = element.GetElement("machine");
			var metadataElement = element.GetElement("metadata");

			Report report = new()
			{
				Id = metadataElement.GetString("id") ?? id ?? json.GetStableHashCode().ToString(),
				Date = metadataElement.GetDateFromUnixTimeSeconds("date"),
				User = metadataElement.GetString("user"),
				CPU = machineElement.Get<CPUInfo>("cpu"),
				OperatingSystem = machineElement.Get<OperatingSystemInfo>("os"),
				System = machineElement.Get<SystemInfo>("system"),
				DockerInfo = machineElement.Get<DockerInfo>("docker")
			};

			var resultsElement = element.GetElement("results");

			List<Result> results = new();

			if (resultsElement.HasValue && resultsElement.Value.ValueKind == JsonValueKind.Array)
			{
				foreach (var resultElement in resultsElement.Value.EnumerateArray())
				{
					results.Add(ParseResultElement(resultElement));
				}
			}

			report.Results = results.ToArray();

			return report;
		}

		private static Result ParseResultElement(JsonElement element)
		{
			var tagsElement = element.GetElement("tags");

			Result result = new()
			{
				Algorithm = tagsElement.HasValue ? tagsElement.GetString("algorithm") : "other",
				Duration = element.GetDouble("duration"),
				Implementation = element.GetString("implementation"),
				IsFaithful = tagsElement.HasValue && tagsElement.GetString("faithful")?.ToLower() == "yes",
				Label = element.GetString("label"),
				Passes = element.GetInt32("passes"),
				Solution = element.GetString("solution"),
				Threads = element.GetInt32("threads")
			};

			if (tagsElement.HasValue && int.TryParse(tagsElement.Value.GetString("bits"), out int bits))
			{
				result.Bits = bits;
			}

			return result;
		}

		public async Task<Report> GetReport(string Id)
		{
			await LoadReportJsonFiles();

			return this.reportMap![Id];
		}

		public async Task<ReportSummary[]> GetSummaries()
		{
			await LoadReportJsonFiles();

			return this.summaries!.ToArray();
		}
	}
}
