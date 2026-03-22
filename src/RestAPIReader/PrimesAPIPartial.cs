using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrimeView.RestAPIReader.Service
{
    public partial class PrimesAPI
    {
        partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
        {
            settings.PropertyNameCaseInsensitive = true;
            settings.Converters.Add(new CustomDateTimeOffsetConverter());
        }

        private class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
        {
            private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                    return default;

                if (DateTimeOffset.TryParseExact(dateString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
                    return result;

                // Fallback to default parsing
                return DateTimeOffset.Parse(dateString, CultureInfo.InvariantCulture);
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
            }
        }
    }
}
