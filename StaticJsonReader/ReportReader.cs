using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeView.StaticJsonReader
{
	public class ReportReader : IReportReader
	{
		List<ReportSummary>? summaries;
		Dictionary<string, Report>? reportMap;
		readonly HttpClient httpClient;
		bool haveJsonFilesLoaded = false;

		public ReportReader(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		private async Task LoadReportJsonFiles()
		{
			if (this.haveJsonFilesLoaded)
				return;

			summaries = new();
			reportMap = new();

			for (int index = 1; ; index++)
			{
				string reportJson;

				try
				{
					reportJson = await this.httpClient.GetStringAsync($"sample-data/report{index}.json");
				}
				catch (HttpRequestException)
				{
					break;
				}

				var reportElement = JsonDocument.Parse(reportJson).RootElement;
				Report report = ParseReportElement(reportJson, reportElement);

				reportMap[report.Id!] = report;
				summaries.Add(ExtractSummary(report));
			}

			haveJsonFilesLoaded = true;
		}

		private static ReportSummary ExtractSummary(Report report)
			=> new()
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
				ResultCount = report.Results?.Length ?? 0
			};

		private static Report ParseReportElement(string json, JsonElement element)
		{
			var machineElement = element.GetElement("machine");

			Report report = new()
			{
				Id = json.GetStableHashCode().ToString(),
				Date = element.GetElement("metadata").GetDateFromUnixTimeSeconds("date"),
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
					results.Add(ParseResultElement(resultElement));
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
					result.Bits = bits;

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
