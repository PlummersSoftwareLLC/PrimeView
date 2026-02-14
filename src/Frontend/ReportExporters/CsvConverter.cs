using CsvHelper;
using CsvHelper.Configuration;
using PrimeView.Entities;
using PrimeView.Frontend.Tools;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace PrimeView.Frontend.ReportExporters
{
    public static class CsvConverter
    {
        private class ResultMap : ClassMap<Result>
        {
            public ResultMap(CultureInfo culture, ILanguageInfoProvider languageInfoProvider)
            {
                AutoMap(culture);
                Map(m => m.Language).Convert(args => args.Value.Language ?? languageInfoProvider.GetLanguageInfo(args.Value.Language).Name);
            }
        }

        public static byte[] Convert(Report report, ILanguageInfoProvider languageInfoProvider)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

            var culture = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(culture)
            {
                Delimiter = ";",
                ShouldQuote = (args) => args.FieldType == typeof(string)
            };

            using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap(new ResultMap(culture, languageInfoProvider));
            csv.WriteRecords(report.Results);

            writer.Flush();
            return memoryStream.ToArray();
        }
    }
}
