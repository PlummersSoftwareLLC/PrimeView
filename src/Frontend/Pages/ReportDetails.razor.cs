using Blazored.LocalStorage;
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
		public ISyncLocalStorageService LocalStorage { get; set; }

		[Inject] 
		public IReportReader ReportReader { get; set; }

		[Inject]
		public IJSInProcessRuntime JSRuntime { get; set; }

		[QueryStringParameter("sc")]
		public string SortColumn { get; set; } = "pp";

		[QueryStringParameter("sd")]
		public bool SortDescending { get; set; } = true;

		[QueryStringParameter("hi")]
		public bool HideSystemInformation { get; set; } = false;

		[QueryStringParameter("hf")]
		public bool HideFilters { get; set; } = false;

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
		private string fi 
		{ 
			get => fi_internal;
			set
			{
				Console.WriteLine(value);
				fi_internal = value;
			}
		}
		private string fi_internal;
		private bool fp_st = true, fp_mt = true;
		private bool fa_ba = true, fa_wh = true, fa_ot = true;
		private bool fb_uk = true, fb_on = true, fb_ot = true;

		protected override async Task OnInitializedAsync()
		{
			report = await ReportReader.GetReport(ReportId);
			await LoadLanguageMap();
		}

		public override Task SetParametersAsync(ParameterView parameters)
		{
			this.SetParametersFromQueryString(NavigationManager, LocalStorage);

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			(string sortColumn, bool sortDescending) = resultTable.GetSortParameterValues();
			Console.WriteLine($"OnAfterRenderAsync: GetSortParameterValues: sortColum = {sortColumn}, sortDescending = {sortDescending}");

			if (!processTableSortingChange && (!SortColumn.EqualsIgnoreCaseOrNull(sortColumn) || SortDescending != sortDescending))
			{
				Console.WriteLine($"SetSortParameterValues: SortColum = {SortColumn}, SortDescending = {SortDescending}");
				if (resultTable.SetSortParameterValues(SortColumn, SortDescending))
					await resultTable.UpdateAsync();
			}

			UpdateQueryString();

			await base.OnAfterRenderAsync(firstRender);
		}

		private void ToggleSystemInfoPanel()
		{
			HideSystemInformation = !HideSystemInformation;

			UpdateQueryString();
		}

		private void ToggleFilterPanel()
		{
			HideFilters = !HideFilters;

			UpdateQueryString();
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

		private void OnTableRefreshStart()
		{
			rowNumber = resultTable.PageNumber * resultTable.PageSize;

			if (!processTableSortingChange)
				return;

			(string sortColumn, bool sortDescending) = resultTable.GetSortParameterValues();
			Console.WriteLine($"OnTableRefreshStart: GetSortParameterValues: sortColum = {sortColumn}, sortDescending = {sortDescending}");

			processTableSortingChange = false;

			bool queryStringUpdateRequired = false;

			if (!sortColumn.EqualsIgnoreCaseOrNull(SortColumn))
			{
				SortColumn = sortColumn;
				queryStringUpdateRequired = true;
			}

			if (sortDescending != SortDescending)
			{
				SortDescending = sortDescending;
				queryStringUpdateRequired = true;
			}

			if (queryStringUpdateRequired)
			{
				Console.WriteLine($"UpdateQueryString: SortColum = {SortColumn}, SortDescending = {SortDescending}");
				UpdateQueryString();
			}
		}

		private LanguageInfo GetLanguageInfo(string language)
			=> languageMap != null && languageMap.ContainsKey(language) ? languageMap[language] : new() { Key = language, Name = language[0].ToString().ToUpper() + language[1..] };

		private void UpdateQueryString()
			=> this.UpdateQueryString(NavigationManager, LocalStorage, JSRuntime);
	}
}
