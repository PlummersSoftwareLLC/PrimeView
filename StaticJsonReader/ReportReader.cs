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
		HttpClient httpClient;
		bool haveJsonFilesLoaded = false;

		public ReportReader(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		private async Task LoadReportJsonFiles()
		{
			if (this.haveJsonFilesLoaded)
				return;

			JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				AllowTrailingCommas = true
			};

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
				var machineElement = reportElement.GetElement("machine");

				Report report = new Report()
				{
					Id = reportJson.GetHashCode().ToString(),
					Date = reportElement.GetElement("metadata").GetDateFromUnixTimeSeconds("date"),
					CPU = machineElement.Get<CPUInfo>("cpu"),
					OperatingSystem = machineElement.Get<OperatingSystemInfo>("os"),
					System = machineElement.Get<SystemInfo>("system"),
					DockerInfo = machineElement.Get<DockerInfo>("docker")
				};

				var resultsElement = reportElement.GetElement("results");
				if (!resultsElement.HasValue || resultsElement.Value.ValueKind != JsonValueKind.Array)
					continue;

				List<Result> results = new();

				foreach (var resultElement in resultsElement.Value.EnumerateArray())
				{
					var tagsElement = resultElement.GetElement("tags");

					Result result = new Result()
					{
						Algorithm = tagsElement.HasValue ? tagsElement.GetString("algorithm") : "other",
						Bits = tagsElement.HasValue ? tagsElement.GetInt32("bits") : null,
						
					};

					results.Add(result);
				}

				report.Results = results.ToArray();

				reportMap[report.Id] = report;
				summaries.Add(ExtractSummary(report));
			}

			haveJsonFilesLoaded = true;
		}

		public async Task<Report> GetReport(string Id)
		{
			throw new NotImplementedException();
		}

		public async Task<ReportSummary[]> GetSummaries()
		{
			throw new NotImplementedException();
		}
	}
}
