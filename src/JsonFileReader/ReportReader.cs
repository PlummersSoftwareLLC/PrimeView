using Microsoft.Extensions.Configuration;
using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly bool isS3Bucket;
		private bool haveJsonFilesLoaded = false;
		private bool reachedMaxFileCount = false;

		public ReportReader(string baseAddress, IConfiguration configuration)
		{
			this.httpClient = new HttpClient { BaseAddress = new Uri(configuration.GetValue(Constants.BaseURI, baseAddress)!) };
			this.indexFileName = configuration.GetValue<string?>(Constants.Index, null);
			this.isS3Bucket = configuration.GetValue(Constants.IsS3Bucket, false);
		}

		private async Task<Report?> LoadReportJsonFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;

			if (haveJsonFilesLoaded && reportMap!.ContainsKey(fileName))
				return reportMap![fileName];

			string reportJson;

			try
			{
				reportJson = await this.httpClient.GetStringAsync(fileName);
			}
			catch
			{
				return null;
			}

			var reportElement = JsonDocument.Parse(reportJson).RootElement;
			return ParseReportElement(reportJson, reportElement, fileName);
		}

		private async Task LoadReportJsonFiles(int maxFileCount)
		{
			if (this.haveJsonFilesLoaded && (!this.reachedMaxFileCount || maxFileCount <= this.reportMap!.Count))
				return;

			string[]? reportFileNames = null;

			if (this.isS3Bucket)
				reportFileNames = await S3BucketIndexReader.GetFileNames(this.httpClient);

			else if (this.indexFileName != null)
			{
				try
				{
					reportFileNames = JsonSerializer.Deserialize<string[]>(await this.httpClient.GetStringAsync(this.indexFileName));
				}
				catch { }
			}

			this.summaries = new();
			this.reportMap = new();
			this.reachedMaxFileCount = false;

			Dictionary<string, Task<string>> stringReaderMap = new();

			for (int fileIndex = 0; fileIndex != reportFileNames?.Length; fileIndex++)
			{
				string fileName = reportFileNames != null ? reportFileNames[fileIndex] : $"data/report{fileIndex + 1}.json";

				stringReaderMap[fileName] = this.httpClient.GetStringAsync(fileName);

				if (--maxFileCount <= 0)
				{
					this.reachedMaxFileCount = true;
					break;
				}
			}

			foreach (var item in stringReaderMap)
			{
				string reportJson;
				string fileName = item.Key;

				try
				{
					reportJson = await item.Value;
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

				try
				{
					var reportElement = JsonDocument.Parse(reportJson).RootElement;
					Report report = ParseReportElement(reportJson, reportElement, fileName);

					this.reportMap[fileName] = report;
					this.summaries.Add(ExtractSummary(report));
				}
				catch
				{
					Console.WriteLine($"Report parsing of file {fileName} failed");
				}
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

		public async Task<Report> GetReport(string id)
		{
			return await LoadReportJsonFile(id) ?? new Report();
		}

		public async Task<ReportSummary[]> GetSummaries(int maxSummaryCount)
		{
			await LoadReportJsonFiles(maxSummaryCount);

			return this.summaries!.Take(maxSummaryCount).ToArray();
		}
	}
}
