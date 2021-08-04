using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PrimeView.Frontend.Tools
{
	public class LanguageInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string URL { get; set; }
		public string Tag { get; set; }

		public class KeyEqualityComparer : IEqualityComparer<LanguageInfo>
		{
			public bool Equals(LanguageInfo x, LanguageInfo y)
				=> (x == null && y == null) || (x != null && y != null && x.Key == y.Key);

			public int GetHashCode([DisallowNull] LanguageInfo obj)
				=> obj.Key.GetHashCode();
		}
	}
}
