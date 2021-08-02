using System.Text.Json.Serialization;

namespace PrimeView.Frontend.Filters
{
	public class ResultFilterPreset
	{
		[JsonPropertyName("nm")]
		public string Name { get; set; }

		[JsonPropertyName("it")]
		public string ImplementationText { get; set; }

		[JsonPropertyName("pt")]
		public string ParallelismText { get; set; }

		[JsonPropertyName("at")]
		public string AlgorithmText { get; set; }

		[JsonPropertyName("ft")]
		public string FaithfulText { get; set; }

		[JsonPropertyName("bt")]
		public string BitsText { get; set; }

		[JsonIgnore]
		public virtual bool IsFixed => false;
	}
}
