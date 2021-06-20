using Microsoft.Extensions.DependencyInjection;
using PrimeView.Entities;
using System;
using System.Net.Http;
using System.Text.Json;

namespace PrimeView.StaticJsonReader
{
	public static class ExtensionMethods
	{
		private static readonly JsonSerializerOptions serializerOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			AllowTrailingCommas = true
		};

		public static int GetStableHashCode(this string str)
		{
			unchecked
			{
				int hash1 = 5381;
				int hash2 = hash1;

				for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ str[i];
					if (i == str.Length - 1 || str[i + 1] == '\0')
						break;
					hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
				}

				return hash1 + (hash2 * 1566083941);
			}
		}

		public static IServiceCollection AddStaticJsonReportReader(this IServiceCollection serviceCollection)
			=> serviceCollection.AddScoped<IReportReader>(sp => new ReportReader(sp.GetRequiredService<HttpClient>()));

		public static T? Get<T>(this JsonElement element)
		{
			try
			{
				return JsonSerializer.Deserialize<T>(element.GetRawText(), serializerOptions);
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

		public static double? GetDouble(this JsonElement? element, string propertyName)
			=> element.HasValue ? GetDouble(element.Value, propertyName) : null;

		public static double? GetDouble(this JsonElement element, string propertyName)
		{
			var childElement = GetElement(element, propertyName);

			return childElement.HasValue && childElement.Value.TryGetDouble(out double value) ? value : null;
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
