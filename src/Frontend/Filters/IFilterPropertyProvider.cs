using System.Collections.Generic;

namespace PrimeView.Frontend.Filters
{
	public interface IFilterPropertyProvider
	{
		public IList<string> FilterLanguages { get; }

		public bool FilterParallelSinglethreaded { get; }
		public bool FilterParallelMultithreaded { get; }

		public bool FilterAlgorithmBase { get; }
		public bool FilterAlgorithmWheel { get; }
		public bool FilterAlgorithmOther { get; }

		public bool FilterFaithful { get; }
		public bool FilterUnfaithful { get; }

		public bool FilterBitsUnknown { get; }
		public bool FilterBitsOne { get; }
		public bool FilterBitsOther { get; }
	}
}
