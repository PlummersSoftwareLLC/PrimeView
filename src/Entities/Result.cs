namespace PrimeView.Entities
{
	public class Result
	{
		public string? Language { get; set; }
		public string? Solution { get; set; }
		public string? Label { get; set; }
		public long? Passes { get; set; }
		public double? Duration { get; set; }
		public int? Threads { get; set; }
		public string? Algorithm { get; set; }
		public bool? IsFaithful { get; set; }
		public int? Bits { get; set; }
		public string? Status { get; set; }

		public double? PassesPerSecond => (double?)Passes / Threads / Duration;
		public bool IsMultiThreaded => Threads > 1;

	}
}
