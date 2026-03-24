using System.Text.Json.Serialization;

namespace HydraWebHook.Models
{
    /// <summary>
    /// Represents a DNS record for External DNS.
    /// </summary>
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