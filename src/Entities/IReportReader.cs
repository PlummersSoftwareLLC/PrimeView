using System.Threading.Tasks;

namespace PrimeView.Entities
{
	public interface IReportReader
	{
		Task<(ReportSummary[] summaries, int total)> GetSummaries(int maxSummaryCount);
		Task<(ReportSummary[] summaries, int total)> GetSummaries(int skipFirst, int maxSummaryCount);
		Task<Report> GetReport(string id);
	}
}
