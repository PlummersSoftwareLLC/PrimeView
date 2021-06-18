using Microsoft.Extensions.DependencyInjection;
using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimeView.StaticJsonReader
{
	public static class ExtensionMethods
	{
		public static IServiceCollection AddStaticJsonReportReader(this IServiceCollection serviceCollection)
			=> serviceCollection.AddSingleton<IReportReader>(sp => new ReportReader(sp.GetRequiredService<HttpClient>()));

		public static T? Get<T>(this JsonElement element)
		{
			try
			{
				return JsonSerializer.Deserialize<T>(element.GetRawText());
			}
			catch { }

			return default;
		}
		 
		public static T? Get<T>(this JsonElement element, string propertyName) where T : class
		  => GetElement(element, propertyName)?.Get<T>();

		public static T? Get<T>(this JsonElement? element, string propertyName) where T : class 
			=> element.HasValue ? Get<T>(element.Value, propertyName) : null;

		public static int? GetInt32(this JsonElement? element, string propertyName)
			=> element.HasValue ? GetInt32(element.Value, propertyName) : null; 

		public static int? GetInt32(this JsonElement element, string propertyName)
		{
			var childElement = GetElement(element, propertyName);

			return childElement.HasValue && childElement.Value.TryGetInt32(out int value) ? value : null;
		}

		public static string? GetString(this JsonElement? element, string propertyName)
			=> element.HasValue ? GetString(element.Value, propertyName) : null;

		public static string? GetString(this JsonElement element, string propertyName)
			=> GetElement(element, propertyName)?.GetString();

		public static DateTime? GetDateFromUnixTimeSeconds(this JsonElement? element, string propertyName)
			=> element.HasValue ? GetDateFromUnixTimeSeconds(element.Value, propertyName) : null;

		public static DateTime? GetDateFromUnixTimeSeconds(this JsonElement element, string propertyName)
		{
			int? seconds = GetInt32(element, propertyName);

			return seconds.HasValue ? DateTimeOffset.FromUnixTimeSeconds(seconds.Value).DateTime : null;
		}

		public static JsonElement? GetElement(this JsonElement element, string propertyName) 
			=> element.TryGetProperty(propertyName, out var childElement) ? childElement : null;

		public static JsonElement? GetElement(this JsonElement? element, string propertyName)
			=> element.HasValue ? GetElement(element.Value, propertyName) : null;
	}
}
