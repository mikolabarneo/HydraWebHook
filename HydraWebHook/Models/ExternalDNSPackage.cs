using System.Text.Json.Serialization;

namespace HydraWebHook.Models
{
    public class ExternalDnsPackage
    {
        [JsonPropertyName("Create")]
        public List<ExternalDnsRecord>? Create { get; set; }

        [JsonPropertyName("UpdateOld")]
        public List<ExternalDnsRecord>? UpdateOld { get; set; }
        [JsonPropertyName("UpdateNew")]
        public List<ExternalDnsRecord>? UpdateNew { get; set; }

        [JsonPropertyName("Delete")]
        public List<ExternalDnsRecord>? Delete { get; set; }
    }
}
