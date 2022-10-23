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

		private string filterRunners = string.Empty;
		private int reportCount;

		[QueryStringParameter("rc")]
		public int ReportCount 
		{ 
			get => this.reportCount; 
			set
			{
				if (value > 0)
					this.reportCount = value;
			} 
		}

		private int skipReports;

		[QueryStringParameter("rs")]
		public int SkipReports 
		{ 
			get => this.skipReports; 
			set 
			{
				if (value >= 0)
					this.skipReports = value;
			} 
		}

		[QueryStringParameter("fr")]
		public string FilterRunners {
			get => filterRunners;
			set
			{
				filterRunners = value ?? string.Empty;

                // The following smells up to high heaven, and I don't like it. Sadly, using a @bind is the only way I
                // could find to reliably link the value that is shown in the runner drop-down (i.e. the selected value)
                // to the one that is actually chosen. And in the context of the @bind, this seems to be the way to kick
                // off a load of the correct summaries and a rerender, in the background.
                if (initialized)
				{
					new Task(async () =>
					{
						await LoadSummaries();
						StateHasChanged();
					}).Start();
				}
			}
		}

		private Runner[] runners = null;
        private ReportSummary[] summaries = null;
		private int totalReports = 0;
		private int newReportCount;
		private int pageNumber = 1;
		private int pageCount = 1;
		private bool initialized = false;

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
			this.runners = await ReportReader.GetRunners();
			SkipReports -= SkipReports % ReportCount;
			await LoadSummaries(); 
			this.newReportCount = ReportCount;
			this.initialized = true;
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
			(this.summaries, this.totalReports) = await ReportReader.GetSummaries(FilterRunners, SkipReports, ReportCount);

			// adjust SkipReports if we're skipping all reports that we have
			if (this.totalReports > 0 && SkipReports >= this.totalReports)
			{
				SkipReports = this.totalReports - 1 - ((this.totalReports - 1) % ReportCount);
				(this.summaries, this.totalReports) = await ReportReader.GetSummaries(FilterRunners, SkipReports, ReportCount);
			}

			this.pageNumber = this.SkipReports / this.ReportCount + 1;
			this.pageCount = this.totalReports / this.ReportCount;
			if (this.totalReports % this.ReportCount > 0)
				this.pageCount++;
		}

		private void LoadReport(string reportId)
		{
			NavigationManager.NavigateTo($"report?id={reportId}");
		}

		private async Task Refresh()
		{
			ReportReader.FlushCache();
			await LoadSummaries();
		}
    }
}
