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

		private async Task ApplyNewReportCount()
		{
			ReportCount = newReportCount;

			if (summaries != null)
				await LoadSummaries();
		}

		private async Task ApplySkipReports(int skipReports)
        {
			SkipReports = skipReports;

			if (summaries != null)
				await LoadSummaries();
		}

		private async Task LoadSummaries()
        {
			(this.summaries, this.totalReports) = await ReportReader.GetSummaries(SkipReports, ReportCount);
        }

		private void LoadReport(string reportId)
		{
			NavigationManager.NavigateTo($"report?id={reportId}");
		}

	}
}
