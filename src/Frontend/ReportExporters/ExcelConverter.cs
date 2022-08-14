using OfficeOpenXml;
using OfficeOpenXml.Style;
using PrimeView.Entities;
using PrimeView.Frontend.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PrimeView.Frontend.ReportExporters
{
	public static class ExcelConverter
	{
		public static byte[] Convert(Report report, ILanguageInfoProvider languageInfoProvider)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using ExcelPackage excelPackage = new();

			var reportSheet = excelPackage.Workbook.Worksheets.Add($"Report");
			FillReportSheet(reportSheet, report);

			var resultSheet = excelPackage.Workbook.Worksheets.Add($"Results");
			FillResultsSheet(resultSheet, report.Results, languageInfoProvider);

			return excelPackage.GetAsByteArray();
		}

		private static void FillReportSheet(ExcelWorksheet sheet, Report report)
		{
			int rowNumber = 1;

			AddGeneralSection(sheet, ref rowNumber, report);
			AddCPUSection(sheet, ref rowNumber, report.CPU);
			AddOperatingSystemSection(sheet, ref rowNumber, report.OperatingSystem);
			AddSystemSection(sheet, ref rowNumber, report.System);
			AddDockerSection(sheet, ref rowNumber, report.DockerInfo);

			sheet.Column(1).Style.Font.Bold = true;
			sheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
			sheet.Columns.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
			sheet.Columns.AutoFit();
		}

		private static void AddGeneralSection(ExcelWorksheet sheet, ref int rowNumber, Report report)
		{
			AddExpandableSection(sheet, ref rowNumber, "General", (sheet, rowNumber) =>
			{
				int sectionTop = rowNumber;
				if (AddValue(sheet, ref rowNumber, "Id", report.Id))
				{
					sheet.Rows[sectionTop, rowNumber - 1].Hidden = true;
				}
				AddValue(sheet, ref rowNumber, "User", report.User);
				AddValue(sheet, ref rowNumber, "Created at", report.Date, format: "yyyy-mm-dd HH:MM:SS");
				return rowNumber;
			});
		}

		private static void AddCPUSection(ExcelWorksheet sheet, ref int rowNumber, CPUInfo cpu)
		{
			AddExpandableSection(sheet, ref rowNumber, "CPU", (sheet, rowNumber) =>
			{
				AddValues(sheet, ref rowNumber, new()
				{
					{ "Manufacturer", cpu.Manufacturer },
					{ "Raspberry processor", cpu.RaspberryProcessor },
					{ "Brand", cpu.Brand },
					{ "Vendor", cpu.Vendor },
					{ "Family", cpu.Family },
					{ "Model", cpu.Model },
					{ "Stepping", cpu.Stepping },
					{ "Revision", cpu.Revision },
					{ "# Cores", cpu.Cores },
					{ "# Efficiency cores", cpu.EfficiencyCores },
					{ "# Performance cores", cpu.PerformanceCores },
					{ "# Physical cores", cpu.PhysicalCores },
					{ "# Processors", cpu.Processors },
					{ "Speed", cpu.Speed },
					{ "Minimum speed", cpu.MinimumSpeed },
					{ "Maximum speed", cpu.MaximumSpeed },
					{ "Voltage", cpu.Voltage },
					{ "Governor", cpu.Governor },
					{ "Socket", cpu.Socket } 
				});
				if (cpu.FlagValues != null)
					AddValue(sheet, ref rowNumber, "Flags", string.Join(", ", cpu.FlagValues.OrderBy(f => f)), wordWrap: true);
				AddValue(sheet, ref rowNumber, "Virtualization", cpu.Virtualization);
				if (cpu.Cache != null && cpu.Cache.Count > 0)
				{
					AddValue(sheet, ref rowNumber, "Cache", force: true);
					foreach (var cacheLine in cpu.Cache)
						AddValue(sheet, ref rowNumber, $"- {cacheLine.Key}", cacheLine.Value);
				}
				return rowNumber;
			});
		}

		private static void AddOperatingSystemSection(ExcelWorksheet sheet, ref int rowNumber, OperatingSystemInfo os)
		{
			AddExpandableValuesSection(sheet, ref rowNumber, "Operating System", new()
			{
				{ "Platform", os.Platform },
				{ "Distribution", os.Distribution },
				{ "Release", os.Release },
				{ "Code name", os.CodeName },
				{ "Kernel", os.Kernel },
				{ "Architecture", os.Architecture },
				{ "Code page", os.CodePage },
				{ "Logo file", os.LogoFile },
				{ "Build", os.Build },
				{ "Service pack", os.ServicePack },
				{ "UEFI", os.IsUefi }
            });
        }

        private static void AddSystemSection(ExcelWorksheet sheet, ref int rowNumber, SystemInfo system)
		{
            AddExpandableValuesSection(sheet, ref rowNumber, "System", new()
            {
				{ "Manufacturer", system.Manufacturer },
				{ "Raspberry manufacturer", system.RaspberryManufacturer },
				{ "SKU", system.SKU },
				{ "Virtual", system.IsVirtual },
				{ "Model", system.Model },
				{ "Version", system.Version },
				{ "Raspberry type", system.RaspberryType },
				{ "Raspberry revision", system.RaspberryRevision }
            });
        }

        private static void AddDockerSection(ExcelWorksheet sheet, ref int rowNumber, DockerInfo docker)
		{
			AddExpandableValuesSection(sheet, ref rowNumber, "Docker", new()
			{
				{ "Kernel version", docker.KernelVersion },
				{ "Kernel version", docker.KernelVersion },
				{ "Operating system", docker.OperatingSystem },
				{ "OS version", docker.OSVersion },
				{ "OS type", docker.OSType },
				{ "Architecture", docker.Architecture },
				{ "# CPUs", docker.CPUCount },
				{ "Total memory", docker.TotalMemory },
				{ "Server version", docker.ServerVersion }
			});
		}

		private static void AddExpandableValuesSection(ExcelWorksheet sheet, ref int rowNumber, string title, List<KeyValuePair<string, object>> entries)
		{
			AddExpandableSection(sheet, ref rowNumber, title, (sheet, rowNumber) =>
			{
				AddValues(sheet, ref rowNumber, entries);
				return rowNumber;
			});
		}

        private static void AddExpandableSection(ExcelWorksheet sheet, ref int rowNumber, string title, Func<ExcelWorksheet, int, int> AddEntries) 
		{
            var cell = sheet.Cells[rowNumber, 1];

            cell.Value = title;
            cell.Style.Font.Size = 14;
            cell.Style.Font.UnderLine = true;

            rowNumber++;
            
			int sectionTop = rowNumber;
			rowNumber = AddEntries(sheet, rowNumber);

			int sectionBottom = rowNumber - 1;
			
			rowNumber++;

            if (sectionBottom <= sectionTop)
                return;

            var rowRange = sheet.Rows[sectionTop, sectionBottom];
            rowRange.OutlineLevel = 1;
            rowRange.Collapsed = false;
        }

		private static bool AddValues(ExcelWorksheet sheet, ref int rowNumber, List<KeyValuePair<string, object>> entries)
		{
			bool result = false;
			foreach (var entry in entries)
				result |= AddValue(sheet, ref rowNumber, entry.Key, entry.Value);
			return result;
		}

		private static bool AddValue(ExcelWorksheet sheet, ref int rowNumber, string label, object value = null, bool force = false, string format = null, bool wordWrap = false)
        {
			if (!force && ((value is string stringValue && string.IsNullOrEmpty(stringValue)) || (value is not string && value == null)))
				return false;

			sheet.Cells[rowNumber, 1].Value = label + ':';
			var valueCell = sheet.Cells[rowNumber, 2];
			valueCell.Value = value;
			if (format != null)
                valueCell.Style.Numberformat.Format = format;
			if (wordWrap)
				valueCell.Style.WrapText = true;

			rowNumber++;
			return true;
        }

		private static void FillResultsSheet(ExcelWorksheet sheet, IEnumerable<Result> results, ILanguageInfoProvider languageInfoProvider)
        {
			sheet.Cells.LoadFromCollection(results);
			int lastRow = sheet.Dimension.Rows - 1;
			for (int i = 2; i <= lastRow; i++)
			{
				var languageInfo = languageInfoProvider.GetLanguageInfo(sheet.Cells[i, Result.LanguageColumnIndex].Text);

				var cell = sheet.Cells[i, Result.LanguageColumnIndex];
				cell.Value = languageInfo.Name;
				ExcelFont font;

				if (!string.IsNullOrEmpty(languageInfo.URL))
				{
					cell.Hyperlink = new Uri(languageInfo.URL);
					font = cell.Style.Font;
					font.Color.SetColor(Color.Blue);
					font.UnderLine = true;
				}

				var uriText = sheet.Cells[i, Result.SolutionUriColumnIndex].Text;
				if (!string.IsNullOrEmpty(uriText))
				{
					cell = sheet.Cells[i, Result.SolutionColumnIndex];
					cell.Hyperlink = new Uri(uriText);
					font = cell.Style.Font;
					font.Color.SetColor(Color.Blue);
					font.UnderLine = true;
				}

				bool allgreen = true;

				cell = sheet.Cells[i, Result.AlgorithmColumnIndex];
				if (cell.Value as string == "base")
					cell.Style.Font.Color.SetColor(Color.Green);
				else
					allgreen = false;

				cell = sheet.Cells[i, Result.IsFaithfulColumnIndex];
				if (cell.Value as bool? ?? false)
					cell.Style.Font.Color.SetColor(Color.Green);
				else
					allgreen = false;

				cell = sheet.Cells[i, Result.BitsColumnIndex];
				if (cell.Value is int && (int)cell.Value == 1)
					cell.Style.Font.Color.SetColor(Color.Green);
				else
					allgreen = false;

				if (allgreen)
					sheet.Cells[i, Result.LabelColumnIndex].Style.Font.Color.SetColor(Color.Green);
			}

			sheet.Cells[2, Result.SolutionColumnIndex, lastRow, Result.SolutionColumnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			sheet.Column(Result.SolutionUriColumnIndex).Hidden = true;
			sheet.Cells[2, Result.IsMultiThreadedColumnIndex, lastRow, Result.IsMultiThreadedColumnIndex].Style.Font.Color.SetColor(Color.Green);
			sheet.Columns.AutoFit();
		}

        public static void Add<T1, T2>(this List<KeyValuePair<T1, T2>> pairList, T1 key, T2 value)
        {
            pairList.Add(new KeyValuePair<T1, T2>(key, value));
        }
    }
}
