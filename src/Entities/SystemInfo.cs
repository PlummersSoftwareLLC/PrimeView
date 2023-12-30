using System.Text.Json.Serialization;

namespace PrimeView.Entities
{
    public class SystemInfo
    {
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Version { get; set; }
        public string? SKU { get; set; }
        public string? RaspberryManufacturer { get; set; }
        public string? RaspberryType { get; set; }
        public string? RaspberryRevision { get; set; }
        [JsonPropertyName("virtual")]
        public bool? IsVirtual { get; set; }
    }
}
