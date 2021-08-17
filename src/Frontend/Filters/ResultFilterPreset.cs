using PrimeView.Frontend.Tools;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PrimeView.Frontend.Filters
{
	public class ResultFilterPreset : IFilterPropertyProvider
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

		[JsonIgnore]
		public IList<string> FilterImplementations
			=> ImplementationText.SplitFilterValues();

		[JsonIgnore]
		public bool FilterParallelSinglethreaded
			=> !ParallelismText.SplitFilterValues().Contains(Constants.SinglethreadedTag);

		[JsonIgnore]
		public bool FilterParallelMultithreaded
			=> !ParallelismText.SplitFilterValues().Contains(Constants.MultithreadedTag);

		[JsonIgnore]
		public bool FilterAlgorithmBase
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.BaseTag);

		[JsonIgnore]
		public bool FilterAlgorithmWheel
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.WheelTag);

		[JsonIgnore]
		public bool FilterAlgorithmOther
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.OtherTag);

		[JsonIgnore]
		public bool FilterFaithful
			=> !FaithfulText.SplitFilterValues().Contains(Constants.FaithfulTag);

		[JsonIgnore]
		public bool FilterUnfaithful
			=> !FaithfulText.SplitFilterValues().Contains(Constants.UnfaithfulTag);

		[JsonIgnore]
		public bool FilterBitsUnknown
			=> !BitsText.SplitFilterValues().Contains(Constants.UnknownTag);

		[JsonIgnore]
		public bool FilterBitsOne
			=> !BitsText.SplitFilterValues().Contains(Constants.OneTag);

		[JsonIgnore]
		public bool FilterBitsOther
			=> !BitsText.SplitFilterValues().Contains(Constants.OtherTag);
	}
}
