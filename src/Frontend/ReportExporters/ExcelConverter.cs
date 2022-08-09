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
	public class ExcelConverter
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

            AddHeader(sheet, ref rowNumber, "General");
            int sectionTop = rowNumber;
            if (AddValue(sheet, ref rowNumber, "Id", report.Id))
            {
                sheet.Rows[sectionTop, rowNumber - 1].Hidden = true;
            }
            AddValue(sheet, ref rowNumber, "User", report.User);
            AddValue(sheet, ref rowNumber, "Created at", report.Date, format: "yyyy-mm-dd HH:MM:SS");
            OutlineSection(sheet, sectionTop, rowNumber - 1);
            rowNumber++;

            AddCPUSection(sheet, ref rowNumber, report.CPU);
            rowNumber++;

			AddOperatingSystemSection(sheet, ref rowNumber, report.OperatingSystem);
			rowNumber++;

			AddSystemSection(sheet, ref rowNumber, report.System);
			rowNumber++;

			AddDockerSection(sheet, ref rowNumber, report.DockerInfo);
			
			sheet.Column(1).Style.Font.Bold = true;
			sheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
			sheet.Columns.AutoFit();
			sheet.Columns.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
		}

        private static void AddCPUSection(ExcelWorksheet sheet, ref int rowNumber, CPUInfo cpu)
        {
            AddHeader(sheet, ref rowNumber, "CPU");
            int sectionTop = rowNumber;
            AddValue(sheet, ref rowNumber, "Manufacturer", cpu.Manufacturer);
            AddValue(sheet, ref rowNumber, "Raspberry processor", cpu.RaspberryProcessor);
            AddValue(sheet, ref rowNumber, "Brand", cpu.Brand);
            AddValue(sheet, ref rowNumber, "Vendor", cpu.Vendor);
            AddValue(sheet, ref rowNumber, "Family", cpu.Family);
            AddValue(sheet, ref rowNumber, "Model", cpu.Model);
            AddValue(sheet, ref rowNumber, "Stepping", cpu.Stepping);
            AddValue(sheet, ref rowNumber, "Revision", cpu.Revision);
            AddValue(sheet, ref rowNumber, "# Cores", cpu.Cores);
            AddValue(sheet, ref rowNumber, "# Efficiency cores", cpu.EfficiencyCores);
            AddValue(sheet, ref rowNumber, "# Performance cores", cpu.PerformanceCores);
            AddValue(sheet, ref rowNumber, "# Physical cores", cpu.PhysicalCores);
            AddValue(sheet, ref rowNumber, "# Processors", cpu.Processors);
            AddValue(sheet, ref rowNumber, "Speed", cpu.Speed);
            AddValue(sheet, ref rowNumber, "Minimum speed", cpu.MinimumSpeed);
            AddValue(sheet, ref rowNumber, "Maximum speed", cpu.MaximumSpeed);
            AddValue(sheet, ref rowNumber, "Voltage", cpu.Voltage);
            AddValue(sheet, ref rowNumber, "Governor", cpu.Governor);
            AddValue(sheet, ref rowNumber, "Socket", cpu.Socket);
            if (cpu.FlagValues != null)
                AddValue(sheet, ref rowNumber, "Flags", string.Join(", ", cpu.FlagValues.OrderBy(f => f)), wordWrap: true);
            AddValue(sheet, ref rowNumber, "Virtualization", cpu.Virtualization);
            if (cpu.Cache != null && cpu.Cache.Count > 0)
            {
                AddValue(sheet, ref rowNumber, "Cache", force: true);
                foreach (var cacheLine in cpu.Cache)
                    AddValue(sheet, ref rowNumber, $"- {cacheLine.Key}", cacheLine.Value);
            }
            OutlineSection(sheet, sectionTop, rowNumber - 1);
		}

        private static void AddOperatingSystemSection(ExcelWorksheet sheet, ref int rowNumber, OperatingSystemInfo os)
        {
            AddHeader(sheet, ref rowNumber, "Operating System");
            int sectionTop = rowNumber;
			AddValue(sheet, ref rowNumber, "Platform", os.Platform);
			AddValue(sheet, ref rowNumber, "Distribution", os.Distribution);
			AddValue(sheet, ref rowNumber, "Release", os.Release);
			AddValue(sheet, ref rowNumber, "Code name", os.CodeName);
			AddValue(sheet, ref rowNumber, "Kernel", os.Kernel);
			AddValue(sheet, ref rowNumber, "Architecture", os.Architecture);
			AddValue(sheet, ref rowNumber, "Code page", os.CodePage);
			AddValue(sheet, ref rowNumber, "Logo file", os.LogoFile);
			AddValue(sheet, ref rowNumber, "Build", os.Build);
			AddValue(sheet, ref rowNumber, "Serive pack", os.ServicePack);
			AddValue(sheet, ref rowNumber, "UEFI", os.IsUefi);
			OutlineSection(sheet, sectionTop, rowNumber - 1);
		}

		private static void AddSystemSection(ExcelWorksheet sheet, ref int rowNumber, SystemInfo system)
		{
			AddHeader(sheet, ref rowNumber, "System");
			int sectionTop = rowNumber;
			AddValue(sheet, ref rowNumber, "Manufacturer", system.Manufacturer);
			AddValue(sheet, ref rowNumber, "Raspberry manufacturer", system.RaspberryManufacturer);
			AddValue(sheet, ref rowNumber, "SKU", system.SKU);
			AddValue(sheet, ref rowNumber, "Virtual", system.IsVirtual);
			AddValue(sheet, ref rowNumber, "Model", system.Model);
			AddValue(sheet, ref rowNumber, "Version", system.Version);
			AddValue(sheet, ref rowNumber, "Raspberry type", system.RaspberryType);
			AddValue(sheet, ref rowNumber, "Raspberry revision", system.RaspberryRevision);
			OutlineSection(sheet, sectionTop, rowNumber - 1);
		}

		private static void AddDockerSection(ExcelWorksheet sheet, ref int rowNumber, DockerInfo docker)
		{
			AddHeader(sheet, ref rowNumber, "Docker");
			int sectionTop = rowNumber;
			AddValue(sheet, ref rowNumber, "Kernel version", docker.KernelVersion);
			AddValue(sheet, ref rowNumber, "Operating system", docker.OperatingSystem);
			AddValue(sheet, ref rowNumber, "OS version", docker.OSVersion);
			AddValue(sheet, ref rowNumber, "OS type", docker.OSType);
			AddValue(sheet, ref rowNumber, "Architecture", docker.Architecture);
			AddValue(sheet, ref rowNumber, "# CPUs", docker.CPUCount);
			AddValue(sheet, ref rowNumber, "Total memory", docker.TotalMemory);
			AddValue(sheet, ref rowNumber, "Server version", docker.ServerVersion);
			OutlineSection(sheet, sectionTop, rowNumber - 1);
		}

        private static void OutlineSection(ExcelWorksheet sheet, int topRow, int bottomRow)
        {
			if (bottomRow <= topRow)
				return;

			var rowRange = sheet.Rows[topRow, bottomRow];
			rowRange.OutlineLevel = 1;
			rowRange.Collapsed = false;
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

		private static bool AddHeader(ExcelWorksheet sheet, ref int rowNumber, string text)
        {
			var cell = sheet.Cells[rowNumber, 1];

			cell.Value = text;
			cell.Style.Font.Size = 14;
			cell.Style.Font.UnderLine = true;

			rowNumber++;
			return true;
        }

		private static void FillResultsSheet(ExcelWorksheet sheet, IEnumerable<Result> results, ILanguageInfoProvider languageInfoProvider)
        {
			sheet.Cells.LoadFromCollection(results);
			for (int i = 2; i < sheet.Dimension.Rows; i++)
			{
				var languageInfo = languageInfoProvider.GetLanguageInfo(sheet.Cells[i, Result.LanguageColumnIndex].Text);

				var cell = sheet.Cells[i, Result.LanguageColumnIndex];
				cell.Value = languageInfo.Name;

				if (!string.IsNullOrEmpty(languageInfo.URL))
				{
					cell.Hyperlink = new Uri(languageInfo.URL);
					cell.Style.Font.Color.SetColor(Color.Blue);
					cell.Style.Font.UnderLine = true;
				}

				if (!string.IsNullOrEmpty(sheet.Cells[i, Result.SolutionUriColumnIndex].Text))
				{
					cell = sheet.Cells[i, Result.SolutionColumnIndex];
					cell.Hyperlink = new Uri(sheet.Cells[i, Result.SolutionUriColumnIndex].Text);
					cell.Style.Font.Color.SetColor(Color.Blue);
					cell.Style.Font.UnderLine = true;
				}
			}

			sheet.Column(Result.SolutionColumnIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
			sheet.Column(Result.SolutionUriColumnIndex).Hidden = true;
			sheet.Columns.AutoFit();
		}
	}
}
