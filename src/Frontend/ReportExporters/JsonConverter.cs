using PrimeView.Entities;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrimeView.Frontend.ReportExporters
{
	public class JsonConverter
	{
		public static byte[] Convert(Report report)
		{
			string jsonValue;
			try
			{
				jsonValue = JsonSerializer.Serialize(report, options: new()
				{
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
					WriteIndented = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});
			}
			catch
			{
				return null;
			}

			return Encoding.UTF8.GetBytes(jsonValue);
		}
	}
}
