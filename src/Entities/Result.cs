using OfficeOpenXml.Attributes;
using OfficeOpenXml.Table;

namespace PrimeView.Entities
{
    [EpplusTable(PrintHeaders = true, ShowTotal = true, TableStyle = TableStyles.Light1)]
    public class Result
    {
        public const int LanguageColumnIndex = 1;
        [EpplusTableColumn(Order = LanguageColumnIndex, TotalsRowLabel = "Count:")]
        public string? Language { get; set; }

        public const int SolutionColumnIndex = 2;
        [EpplusTableColumn(Order = SolutionColumnIndex, TotalsRowFunction = RowFunctions.Count)]
        public string? Solution { get; set; }

        public const int SolutionUriColumnIndex = 3;
        [EpplusTableColumn(Order = SolutionUriColumnIndex, Header = "Solution link")]
        public string? SolutionUrl { get; set; }

        public const int LabelColumnIndex = 4;
        [EpplusTableColumn(Order = LabelColumnIndex)]
        public string? Label { get; set; }

        public const int IsMultiThreadedColumnIndex = 5;
        [EpplusTableColumn(Order = IsMultiThreadedColumnIndex, Header = "Multithreaded?")]
        public bool IsMultiThreaded => Threads > 1;

        public const int PassesColumnIndex = 6;
        [EpplusTableColumn(Order = PassesColumnIndex, Header = "Number of passes")]
        public long? Passes { get; set; }

        public const int DurationColumnIndex = 7;
        [EpplusTableColumn(Order = DurationColumnIndex)]
        public double? Duration { get; set; }

        public const int ThreadsColumnIndex = 8;
        [EpplusTableColumn(Order = ThreadsColumnIndex, Header = "Number of threads")]
        public int? Threads { get; set; }

        public const int PassesPerSecondColumnIndex = 9;
        [EpplusTableColumn(Order = PassesPerSecondColumnIndex, Header = "Passes / thread / second")]
        public double? PassesPerSecond => (double?)Passes / Threads / Duration;

        public const int AlgorithmColumnIndex = 10;
        [EpplusTableColumn(Order = AlgorithmColumnIndex)]
        public string? Algorithm { get; set; }

        public const int IsFaithfulColumnIndex = 11;
        [EpplusTableColumn(Order = IsFaithfulColumnIndex, Header = "Faithful?")]
        public bool? IsFaithful { get; set; }

        public const int BitsColumnIndex = 12;
        [EpplusTableColumn(Order = BitsColumnIndex, Header = "Bits per prime")]
        public int? Bits { get; set; }

        [EpplusIgnore]
        public string? Status { get; set; }
    }
}
