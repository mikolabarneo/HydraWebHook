using System.Text.Json.Serialization;

namespace HydraWebHook.Models
{
    public class ExternalDnsRecord
    {
        [JsonPropertyName("dnsName")]
        public required string DnsName { get; set; }

        [JsonPropertyName("targets")]
        public required List<string> Targets { get; set; }

        [JsonPropertyName("recordType")]
        public required string RecordType { get; set; }

        [JsonPropertyName("recordTTL")]
        public int RecordTtl { get; set; }
    }
}