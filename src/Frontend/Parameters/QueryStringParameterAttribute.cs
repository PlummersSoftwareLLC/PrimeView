using System;

namespace PrimeView.Frontend.Parameters
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class QueryStringParameterAttribute : Attribute
	{
		public QueryStringParameterAttribute() {}

		public QueryStringParameterAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}
