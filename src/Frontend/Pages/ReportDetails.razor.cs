using Blazored.LocalStorage;
using BlazorTable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

		[QueryStringParameter("fi")]
		public string FilterImplementationText { get; set; } = string.Empty;

		[QueryStringParameter("fp")]
		public string FilterParallelismText
		{
			get => GetFilterValueString(!FilterParallelSinglethreaded, "st", !FilterParallelMultithreaded, "mt");

			set
			{
				value = $"~{value}~";

				FilterParallelSinglethreaded = !value.Contains("~st~");
				FilterParallelMultithreaded = !value.Contains("~mt~");
			}
		}

		[QueryStringParameter("fa")]
		public string FilterAlgorithmText
		{
			get => GetFilterValueString(!FilterAlgorithmBase, "ba", !FilterAlgorithmWheel, "wh", !FilterAlgorithmOther, "ot");

			set
			{
				value = $"~{value}~";

				FilterAlgorithmBase = !value.Contains("~ba~");
				FilterAlgorithmWheel = !value.Contains("~wh~");
				FilterAlgorithmOther = !value.Contains("~ot~");
			}
		}

		[QueryStringParameter("ff")]
		public string FilterFaithfulText
		{
			get => GetFilterValueString(!FilterFaithful, "ff", !FilterUnfaithful, "uf");

			set
			{
				value = $"~{value}~";

				FilterFaithful = !value.Contains("~ff~");
				FilterUnfaithful = !value.Contains("~uf~");
			}
		}

		[QueryStringParameter("fb")]
		public string FilterBitsText
		{
			get => GetFilterValueString(!FilterBitsUnknown, "uk", !FilterBitsOne, "on", !FilterBitsOther, "ot");

			set
			{
				value = $"~{value}~";

				FilterBitsUnknown = !value.Contains("~uk~");
				FilterBitsOne = !value.Contains("~on~");
				FilterBitsOther = !value.Contains("~ot~");
			}
		}

		public IList<string> FilterImplementations
			=> FilterImplementationText.Split("~", StringSplitOptions.RemoveEmptyEntries);

		public bool FilterParallelSinglethreaded { get; set; } = true;
		public bool FilterParallelMultithreaded { get; set; } = true;

		public bool FilterAlgorithmBase { get; set; } = true;
		public bool FilterAlgorithmWheel { get; set; } = true;
		public bool FilterAlgorithmOther { get; set; } = true;

		public bool FilterFaithful { get; set; } = true;
		public bool FilterUnfaithful { get; set; } = true;

		public bool FilterBitsUnknown { get; set; } = true;
		public bool FilterBitsOne { get; set; } = true;
		public bool FilterBitsOther { get; set; } = true;

		private bool AreFiltersClear
			=> FilterImplementationText == string.Empty
				&& FilterParallelismText == string.Empty
				&& FilterAlgorithmText == string.Empty
				&& FilterFaithfulText == string.Empty
				&& FilterBitsText == string.Empty;

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

		private ElementReference implementationsSelect;

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

			if (!processTableSortingChange && (!SortColumn.EqualsIgnoreCaseOrNull(sortColumn) || SortDescending != sortDescending))
			{
				if (resultTable.SetSortParameterValues(SortColumn, SortDescending))
					await resultTable.UpdateAsync();
			}

			UpdateQueryString();

			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task ClearFilters()
		{
			FilterImplementationText = string.Empty;
			FilterParallelismText = string.Empty;
			FilterAlgorithmText = string.Empty;
			FilterFaithfulText = string.Empty;
			FilterBitsText = string.Empty;

			await JSRuntime.InvokeVoidAsync("PrimeViewJS.ClearMultiselectValues", implementationsSelect);
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
				UpdateQueryString();
		}

		private LanguageInfo GetLanguageInfo(string language)
			=> languageMap != null && languageMap.ContainsKey(language) ? languageMap[language] : new() { Key = language, Name = language[0].ToString().ToUpper() + language[1..] };

		private void UpdateQueryString()
			=> this.UpdateQueryString(NavigationManager, LocalStorage, JSRuntime);

		private async Task ImplementationSelectionChanged(EventArgs args)
		{
			FilterImplementationText = await JSRuntime.InvokeAsync<string>("PrimeViewJS.GetMultiselectValues", implementationsSelect, "~") ?? string.Empty;
		}

		private string GetFilterValueString(params object[] flagSet)
		{
			List<string> setFlags = new(); 

			for (int i = 0; i < flagSet.Length; i += 2)
			{
				if ((bool)flagSet[i])
					setFlags.Add(flagSet[i + 1].ToString());
			}

			return setFlags.Count > 0 ? string.Join("~", setFlags) : string.Empty;
		}
	}
}
