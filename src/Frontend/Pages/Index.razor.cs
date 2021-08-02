using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PrimeView.Entities;
using PrimeView.Frontend.Parameters;
using PrimeView.Frontend.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public int MaxReportCount { get; set; }

		private ReportSummary[] summaries = null;
		private int newMaxReportCount; 

		public override Task SetParametersAsync(ParameterView parameters)
		{
			MaxReportCount = Configuration.GetValue(Constants.MaxReportCount, 30);
			SortColumn = "dt";
			SortDescending = true;

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnInitializedAsync()
		{
			this.summaries = await ReportReader.GetSummaries(MaxReportCount);
			this.newMaxReportCount = MaxReportCount;
		}

		private async Task ApplyNewMaxReportCount()
		{
			MaxReportCount = newMaxReportCount;

			if (summaries != null)
				summaries = await ReportReader.GetSummaries(MaxReportCount);
		}

		private void LoadReport(string reportId)
		{
			NavigationManager.NavigateTo($"report?id={reportId}");
		}

	}
}
