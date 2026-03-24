using System.Text.Json.Serialization;

namespace HydraWebHook.Models
{
    /// <summary>
    /// Represents a package of DNS changes (create, update, delete) for External DNS.
    /// </summary>
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
