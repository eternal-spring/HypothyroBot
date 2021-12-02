using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class EntityModel
    {
        [JsonProperty("tokens")]
        public object Tokens { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}