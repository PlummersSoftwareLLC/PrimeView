using BlazorTable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using PrimeView.Entities;
using PrimeView.Frontend.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Pages
{
	public partial class ReportDetails
	{
		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public HttpClient Http { get; set; }

		[Inject] 
		public IReportReader ReportReader { get; set; }

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		[QueryStringParameter("sc")]
		public string SortColumn { get; set; }

		[QueryStringParameter("sd")]
		public string SortDescending { get; set; }

		[QueryStringParameter("id")]
		public string ReportId { get; set; }

		private string ReportTitle
		{
			get
			{
				StringBuilder titleBuilder = new();

				if (report?.User != null)
					titleBuilder.Append($" by {report.User}");

				if (report?.Date != null)
					titleBuilder.Append($" at {report.Date.Value.ToLocalTime()}");

				return titleBuilder.Length > 0 ? $"Report generated{titleBuilder}" : "Report";
			}
		}

		private Table<Result> resultTable;
		private Report report = null;
		private int rowNumber = 0;
		private Dictionary<string, LanguageInfo> languageMap = null;
		private bool processTableSortingChange = false;

		protected override async Task OnInitializedAsync()
		{
			report = await ReportReader.GetReport(ReportId);
			await LoadLanguageMap();
		}

		public override Task SetParametersAsync(ParameterView parameters)
		{
			this.SetParametersFromQueryString(NavigationManager);

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (ApplyOrUpdateSortParameters())
			{
				await resultTable.UpdateAsync();
				resultTable.Refresh();
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task LoadLanguageMap()
		{
			try
			{
				languageMap = await Http.GetFromJsonAsync<Dictionary<string, LanguageInfo>>("data/langmap.json");
				foreach (var entry in languageMap)
					entry.Value.Key = entry.Key;
			}
			catch	{}
		}

		private bool ApplyOrUpdateSortParameters()
		{
			(string sortColumn, string sortDescending) = resultTable.GetSortParameterValues();
			Console.WriteLine($"GetSortParameterValues: sortColum = {sortColumn}, sortDescending = {sortDescending}");

			if (!processTableSortingChange)
			{
				if (!SortColumn.EqualsIgnoreCaseOrNull(sortColumn) || !SortDescending.EqualsIgnoreCaseOrNull(sortDescending))
				{
					Console.WriteLine($"SetSortParameterValues: SortColum = {SortColumn}, SortDescending = {SortDescending}");
					return resultTable.SetSortParameterValues(SortColumn, SortDescending);
				}

				return false;
			}

			processTableSortingChange = false;

			bool queryStringUpdateRequired = false;

			if (!sortColumn.EqualsIgnoreCaseOrNull(SortColumn))
			{
				SortColumn = sortColumn;
				queryStringUpdateRequired = true;
			}

			if (!sortDescending.EqualsIgnoreCaseOrNull(SortDescending))
			{
				SortDescending = sortDescending;
				queryStringUpdateRequired = true;
			}

			if (queryStringUpdateRequired)
			{
				Console.WriteLine($"UpdateQueryString: SortColum = {SortColumn}, SortDescending = {SortDescending}");
				this.UpdateQueryString(NavigationManager, JSRuntime);
			}

			return false;
		}

		private void OnTableRefreshStart()
		{
			rowNumber = resultTable.PageNumber * resultTable.PageSize;

			ApplyOrUpdateSortParameters();
		}

		private LanguageInfo GetLanguageInfo(string language)
			=> languageMap != null && languageMap.ContainsKey(language) ? languageMap[language] : new() { Key = language, Name = language[0].ToString().ToUpper() + language[1..] };
	}
}
