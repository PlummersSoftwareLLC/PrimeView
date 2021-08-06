using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using PrimeView.Entities;
using PrimeView.Frontend.Filters;
using PrimeView.Frontend.Parameters;
using PrimeView.Frontend.Sorting;
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
	public partial class ReportDetails : SortedTablePage<Result>, IFilterPropertyProvider, ILanguageInfoProvider
	{
		private const string FilterPresetStorageKey = "ResultFilterPresets";

		[Inject]
		public HttpClient Http { get; set; }

		[Inject]
		public IConfiguration Configuration { get; set; }

		[Inject]
		public IReportReader ReportReader { get; set; }

		[QueryStringParameter("hi")]
		public bool HideSystemInformation { get; set; } = false;

		[QueryStringParameter("hf")]
		public bool HideFilters { get; set; } = false;

		[QueryStringParameter("hp")]
		public bool HideFilterPresets { get; set; } = false;

		[QueryStringParameter("id")]
		public string ReportId { get; set; }

		[QueryStringParameter("fi")]
		public string FilterImplementationText { get; set; } = string.Empty;

		[QueryStringParameter("fp")]
		public string FilterParallelismText
		{
			get => JoinFilterValueString(!FilterParallelSinglethreaded, Constants.SinglethreadedTag, !FilterParallelMultithreaded, Constants.MultithreadedTag);

			set
			{
				var values = value.SplitFilterValues();

				FilterParallelSinglethreaded = !values.Contains(Constants.SinglethreadedTag);
				FilterParallelMultithreaded = !values.Contains(Constants.MultithreadedTag);
			}
		}

		[QueryStringParameter("fa")]
		public string FilterAlgorithmText
		{
			get => JoinFilterValueString(!FilterAlgorithmBase, Constants.BaseTag, !FilterAlgorithmWheel, Constants.WheelTag, !FilterAlgorithmOther, Constants.OtherTag);

			set
			{
				var values = value.SplitFilterValues();

				FilterAlgorithmBase = !values.Contains(Constants.BaseTag);
				FilterAlgorithmWheel = !values.Contains(Constants.WheelTag);
				FilterAlgorithmOther = !values.Contains(Constants.OtherTag);
			}
		}

		[QueryStringParameter("ff")]
		public string FilterFaithfulText
		{
			get => JoinFilterValueString(!FilterFaithful, Constants.FaithfulTag, !FilterUnfaithful, Constants.UnfaithfulTag);

			set
			{
				var values = value.SplitFilterValues();

				FilterFaithful = !values.Contains(Constants.FaithfulTag);
				FilterUnfaithful = !values.Contains(Constants.UnfaithfulTag);
			}
		}

		[QueryStringParameter("fb")]
		public string FilterBitsText
		{
			get => JoinFilterValueString(!FilterBitsUnknown, Constants.UnknownTag, !FilterBitsOne, Constants.OneTag, !FilterBitsOther, Constants.OtherTag);

			set
			{
				var values = value.SplitFilterValues();

				FilterBitsUnknown = !values.Contains(Constants.UnknownTag);
				FilterBitsOne = !values.Contains(Constants.OneTag);
				FilterBitsOther = !values.Contains(Constants.OtherTag);
			}
		}

		[QueryStringParameter("tp")]
		public bool OnlyHighestPassesPerSecondPerThreadPerLanguage { get; set; } = false;

		public IList<string> FilterImplementations
			=> FilterImplementationText.SplitFilterValues();

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

		private string solutionUrlTemplate;
		private Report report = null;
		private int rowNumber = 0;
		private Dictionary<string, LanguageInfo> languageMap = null;
		private List<ResultFilterPreset> filterPresets = null;
		private string filterPresetName;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Used as @ref in razor file")]
		private ElementReference implementationsSelect;

		public override Task SetParametersAsync(ParameterView parameters)
		{
			SortColumn = "pp";
			SortDescending = true;

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnInitializedAsync()
		{
			this.solutionUrlTemplate = Configuration.GetValue<string>(Constants.SolutionUrlTemplate, null);
			this.report = await ReportReader.GetReport(ReportId);
			await LoadLanguageMap();

			if (LocalStorage.ContainKey(FilterPresetStorageKey))
			{
				try
				{
					this.filterPresets = LocalStorage.GetItem<List<ResultFilterPreset>>(FilterPresetStorageKey);
				}
				catch
				{
					LocalStorage.RemoveItem(FilterPresetStorageKey);
				}

			}

			if (this.filterPresets == null)
				this.filterPresets = new();

			InsertFilterPreset(new LeaderboardFilterPreset());
			InsertFilterPreset(new MultithreadedLeaderboardFilterPreset());

			await base.OnInitializedAsync();
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
		}

		private void ToggleFilterPanel()
		{
			HideFilters = !HideFilters;
		}

		private void ToggleFilterPresetPanel()
		{
			HideFilterPresets = !HideFilterPresets;
		}

		private async Task LoadLanguageMap()
		{
			try
			{
				this.languageMap = await Http.GetFromJsonAsync<Dictionary<string, LanguageInfo>>("data/langmap.json");
				foreach (var entry in languageMap)
				{
					entry.Value.Key = entry.Key;
				}
			}
			catch { }
		}

		protected override void OnTableRefreshStart()
		{
			rowNumber = this.sortedTable.PageNumber * this.sortedTable.PageSize;

			base.OnTableRefreshStart();
		}

		public LanguageInfo GetLanguageInfo(string language)
		{
			if (this.languageMap == null)
				this.languageMap = new();

			if (languageMap.ContainsKey(language))
				return this.languageMap[language];

			LanguageInfo info = new() { Key = language, Name = language[0].ToString().ToUpper() + language[1..] };

			this.languageMap[language] = info;

			return info;
		}

		private async Task ImplementationSelectionChanged()
		{
			FilterImplementationText = await JSRuntime.InvokeAsync<string>("PrimeViewJS.GetMultiselectValues", implementationsSelect, "~") ?? string.Empty;
		}

		private static string JoinFilterValueString(params object[] flagSet)
		{
			List<string> setFlags = new();

			for (int i = 0; i < flagSet.Length; i += 2)
			{
				if ((bool)flagSet[i])
					setFlags.Add(flagSet[i + 1].ToString());
			}

			return setFlags.JoinFilterValues();
		}

		private bool IsFilterPresetNameValid(string name)
			=> !string.IsNullOrWhiteSpace(name)
				&& (filterPresets == null || !filterPresets.Any(preset => preset.IsFixed && string.Equals(preset.Name, name, StringComparison.OrdinalIgnoreCase)));

		private async Task ApplyFilterPreset(int index)
		{
			var preset = this.filterPresets?[index];

			if (preset == null)
				return;

			FilterAlgorithmText = preset.AlgorithmText;
			FilterBitsText = preset.BitsText;
			FilterFaithfulText = preset.FaithfulText;
			FilterImplementationText = preset.ImplementationText;
			FilterParallelismText = preset.ParallelismText;

			var filterImplementations = FilterImplementations;

			if (filterImplementations.Count > 0)
				await JSRuntime.InvokeVoidAsync("PrimeViewJS.SetMultiselectValues", implementationsSelect, FilterImplementations.ToArray());

			else
				await JSRuntime.InvokeVoidAsync("PrimeViewJS.ClearMultiselectValues", implementationsSelect);

			this.filterPresetName = preset.IsFixed ? string.Empty : preset.Name;
		}

		private void RemoveFilterPreset(int index)
		{
			this.filterPresets?.RemoveAt(index);

			SaveFilterPresets();
		}

		private void InsertFilterPreset(ResultFilterPreset preset)
		{
			if (this.filterPresets == null)
				this.filterPresets = new();

			int i;
			for (i = 0; i < this.filterPresets.Count && string.Compare(preset.Name, this.filterPresets[i].Name, StringComparison.OrdinalIgnoreCase) > 0; i++) ;

			if (i < this.filterPresets.Count && string.Equals(preset.Name, this.filterPresets[i].Name, StringComparison.OrdinalIgnoreCase))
			{
				if (this.filterPresets[i].IsFixed)
					return;

				this.filterPresets.RemoveAt(i);
			}

			this.filterPresets.Insert(i, preset);

			SaveFilterPresets();
		}

		private void AddFilterPreset()
		{
			if (string.IsNullOrWhiteSpace(this.filterPresetName))
				return;

			this.filterPresetName = this.filterPresetName.Trim();

			InsertFilterPreset(new()
			{
				Name = this.filterPresetName,
				AlgorithmText = FilterAlgorithmText,
				BitsText = FilterBitsText,
				FaithfulText = FilterFaithfulText,
				ImplementationText = FilterImplementationText,
				ParallelismText = FilterParallelismText
			});

			this.filterPresetName = null;
		}

		private void SaveFilterPresets()
		{
			LocalStorage.SetItem(FilterPresetStorageKey, filterPresets.Where(preset => !preset.IsFixed));
		}
	}
}
