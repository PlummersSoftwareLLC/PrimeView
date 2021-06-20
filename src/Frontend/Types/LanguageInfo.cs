using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Types
{
	public class LanguageInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string URL { get; set; }

		public class KeyEqualityComparer : IEqualityComparer<LanguageInfo>
		{
			public bool Equals(LanguageInfo x, LanguageInfo y)
				=> (x == null && y == null) || (x != null && y != null && x.Key == y.Key);

			public int GetHashCode([DisallowNull] LanguageInfo obj)
				=> obj.Key.GetHashCode();
		}
	}
}
