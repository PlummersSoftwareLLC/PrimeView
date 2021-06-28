using Blazored.LocalStorage;
using BlazorTable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PrimeView.Frontend.Parameters;
using System;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Sorting
{
	public partial class SortedTablePage<TableItem> : ComponentBase
	{
		[Inject]
		public NavigationManager NavigationManager { get; set; }

		[Inject]
		public ISyncLocalStorageService LocalStorage { get; set; }

		[Inject]
		public IJSInProcessRuntime JSRuntime { get; set; }

		[QueryStringParameter("sc")]
		public string SortColumn { get; set; } = string.Empty;

		[QueryStringParameter("sd")]
		public bool SortDescending { get; set; } = false;

		protected Table<TableItem> sortedTable;

		private bool processTableSortingChange = false;

		public override Task SetParametersAsync(ParameterView parameters)
		{
			this.SetParametersFromQueryString(NavigationManager, LocalStorage);

			return base.SetParametersAsync(parameters);
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			(string sortColumn, bool sortDescending) = sortedTable.GetSortParameterValues();

			if (!processTableSortingChange && (!SortColumn.EqualsIgnoreCaseOrNull(sortColumn) || SortDescending != sortDescending))
			{
				if (sortedTable.SetSortParameterValues(SortColumn, SortDescending))
					await sortedTable.UpdateAsync();
			}

			UpdateQueryString();

			await base.OnAfterRenderAsync(firstRender);
		}

		protected void FlagTableSortingChange()
		{
			processTableSortingChange = true;
		}

		protected virtual void OnTableRefreshStart()
		{
			if (!processTableSortingChange)
				return;

			(string sortColumn, bool sortDescending) = sortedTable.GetSortParameterValues();

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

		protected virtual void UpdateQueryString()
		{
			this.UpdateQueryString(NavigationManager, LocalStorage, JSRuntime);
		}
	}
}
