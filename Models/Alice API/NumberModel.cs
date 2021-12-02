using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    internal class NumberModel : EntityModel
    {
        [JsonProperty("value")]
        public double Value { get; set; }
    }
}