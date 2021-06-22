using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeView.Frontend.QueryStringParameters
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class QueryStringParameterAttribute : Attribute
  {
    public QueryStringParameterAttribute()
    {
    }

    public QueryStringParameterAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}
