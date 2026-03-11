using System.Text.Json.Serialization;

namespace HydraWebHook.Models
{
    public class DomainFilter
    {
        [JsonPropertyName("Filter")]
        public List<string> Filter { get; set; } = ["shore.ox.ac.uk"];

        [JsonPropertyName("exclude")]
        public List<string>? Exclude { get; set; }

        [JsonPropertyName("regex")]
        public string? Regex { get; set; }

        [JsonPropertyName("regexExclusion")]
        public string? RegexExclusion { get; set; }
    }
}
