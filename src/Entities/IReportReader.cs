using System.Threading.Tasks;

namespace PrimeView.Entities
{
	public interface IReportReader
	{
		Task<ReportSummary[]> GetSummaries();
		Task<Report> GetReport(string Id);
	}
}
