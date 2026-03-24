namespace HydraWebHook.Models
{
    /// <summary>
    /// Represents the configuration options for the Hydra client.
    /// </summary>
    public class HydraOptions
    {
        public string Uri { get; set; } = "";
        public string TokenName { get; set; } = "";
        public string TokenPass { get; set; } = "";
    }
}
