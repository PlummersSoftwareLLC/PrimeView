using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Tools
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
	}
}
