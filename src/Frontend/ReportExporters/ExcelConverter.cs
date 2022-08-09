using OfficeOpenXml;
using OfficeOpenXml.Style;
using PrimeView.Entities;
using PrimeView.Frontend.Tools;
using System;
using System.Drawing;

namespace PrimeView.Frontend.ReportExporters
{
	public class ExcelConverter
	{
		public static byte[] Convert(Report report, ILanguageInfoProvider languageInfoProvider)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using ExcelPackage excelPackage = new();

			string tag = $"{report.User ?? string.Empty} {(report.Date.HasValue ? report.Date.Value.ToString() : string.Empty)}".Replace('/', '_').Replace(':', '_');
			tag = !string.IsNullOrWhiteSpace(tag) ? $" - {tag}" : string.Empty;

			var resultSheet = excelPackage.Workbook.Worksheets.Add($"Results{tag}");

			resultSheet.Cells.LoadFromCollection(report.Results);
			for (int i = 2; i < resultSheet.Dimension.Rows; i++)
			{
				var languageInfo = languageInfoProvider.GetLanguageInfo(resultSheet.Cells[i, Result.LanguageColumnIndex].Text);

				var cell = resultSheet.Cells[i, Result.LanguageColumnIndex];
				cell.Value = languageInfo.Name;

				if (!string.IsNullOrEmpty(languageInfo.URL))
				{
					cell.Hyperlink = new Uri(languageInfo.URL);
					cell.Style.Font.Color.SetColor(Color.Blue);
					cell.Style.Font.UnderLine = true;
				}

				if (!string.IsNullOrEmpty(resultSheet.Cells[i, Result.SolutionUriColumnIndex].Text))
				{
					cell = resultSheet.Cells[i, Result.SolutionColumnIndex];
					cell.Hyperlink = new Uri(resultSheet.Cells[i, Result.SolutionUriColumnIndex].Text);
					cell.Style.Font.Color.SetColor(Color.Blue);
					cell.Style.Font.UnderLine = true;
				}
			}

			resultSheet.Column(Result.SolutionUriColumnIndex).Hidden = true;
			resultSheet.Columns.AutoFit();

			return excelPackage.GetAsByteArray();
		}
	}
}
