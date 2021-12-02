using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class MarkupModel
    {
        [JsonProperty("dangerous_context")]
        public bool DangerousContext { get; set; }
    }
}