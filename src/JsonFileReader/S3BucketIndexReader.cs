using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PrimeView.JsonFileReader
{
	static class S3BucketIndexReader
	{
		public static async Task<string[]?> GetFileNames(HttpClient httpClient)
		{
			XElement indexElement;

			try
			{
				indexElement = XElement.Parse(await httpClient.GetStringAsync(""));
			}
			catch 
			{
				return null;
			}

			XNamespace ns = indexElement.GetDefaultNamespace();

			return indexElement.Descendants(ns + "Contents")?
				.Select(element => new KeyValuePair<string, DateTime>(element.Element(ns + "Key")!.Value, XmlConvert.ToDateTime(element.Element(ns + "LastModified")!.Value, XmlDateTimeSerializationMode.Utc)))
				.OrderByDescending(pair => pair.Value)
				.Select(pair => pair.Key)
				.ToArray();
		}
	}
}
