using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PrimeView.Entities;
using PrimeView.Frontend.Parameters;
using PrimeView.Frontend.Tools;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Pages
{
	public partial class Index
	{
		[Inject]
		public IConfiguration Configuration { get; set; }

		[Inject]
		public IReportReader ReportReader { get; set; }

		[QueryStringParameter("rc")]
		public int ReportCount { get; set; }

		[QueryStringParameter("rs")]
		public int SkipReports { get; set; }

		private ReportSummary[] summaries = null;
		private int totalReports = 0;
		private int newReportCount;
		private int pageNumber = 1;
		private int pageCount = 1;

		public override Task SetParametersAsync(ParameterView parameters)
		{
			ReportCount = Configuration.GetValue(Constants.ReportCount, 50);
			SkipReports = 0;
			SortColumn = "dt";
			SortDescending = true;

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnInitializedAsync()
		{
			SkipReports -= SkipReports % ReportCount;
			await LoadSummaries(); 
			this.newReportCount = ReportCount;
		}

		private async Task ApplyNewReportCount(int reportCount)
        {
			this.newReportCount = reportCount;
			await ApplyNewReportCount();
        }

		private async Task ApplyNewReportCount()
		{
			ReportCount = this.newReportCount;
			SkipReports -= SkipReports % ReportCount;

			if (summaries != null)
				await LoadSummaries();
		}

		private async Task ApplyPageNumber(int pageNumber)
        {
			SkipReports = (pageNumber - 1) * ReportCount;

			if (summaries != null)
				await LoadSummaries();
		}

		private async Task LoadSummaries()
        {
			(this.summaries, this.totalReports) = await ReportReader.GetSummaries(SkipReports, ReportCount);

			this.pageNumber = this.SkipReports / this.ReportCount + 1;
			this.pageCount = this.totalReports / this.ReportCount;
			if (this.totalReports % this.ReportCount > 0)
				this.pageCount++;
		}

		private void LoadReport(string reportId)
		{
			NavigationManager.NavigateTo($"report?id={reportId}");
		}

	}
}
