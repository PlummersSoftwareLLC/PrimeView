using PrimeView.Frontend.Tools;
using System.Collections.Generic;
using System.Linq;
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

		public IList<string> FilterImplementations
			=> ImplementationText.SplitFilterValues();

		public bool FilterParallelSinglethreaded 
			=> !ParallelismText.SplitFilterValues().Contains(Constants.SinglethreadedTag);

		public bool FilterParallelMultithreaded
			=> !ParallelismText.SplitFilterValues().Contains(Constants.MultithreadedTag);

		public bool FilterAlgorithmBase 
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.BaseTag);

		public bool FilterAlgorithmWheel
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.WheelTag);

		public bool FilterAlgorithmOther
			=> !AlgorithmText.SplitFilterValues().Contains(Constants.OtherTag);

		public bool FilterFaithful
			=> !FaithfulText.SplitFilterValues().Contains(Constants.FaithfulTag);

		public bool FilterUnfaithful
			=> !FaithfulText.SplitFilterValues().Contains(Constants.UnfaithfulTag);

		public bool FilterBitsUnknown
			=> !BitsText.SplitFilterValues().Contains(Constants.UnknownTag);

		public bool FilterBitsOne
			=> !BitsText.SplitFilterValues().Contains(Constants.OneTag);

		public bool FilterBitsOther
			=> !BitsText.SplitFilterValues().Contains(Constants.OtherTag);
	}
}
