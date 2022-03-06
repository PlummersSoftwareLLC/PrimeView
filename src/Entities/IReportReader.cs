using System.Threading.Tasks;

namespace PrimeView.Entities
{
	public interface IReportReader
	{
		Task<ReportSummary[]> GetSummaries(int maxSummaryCount);
		Task<ReportSummary[]> GetSummaries(int skipFirst, int maxSummaryCount);
		Task<Report> GetReport(string id);
	}
}
