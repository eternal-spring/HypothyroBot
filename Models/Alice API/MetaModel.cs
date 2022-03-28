using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class MetaModel
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        [JsonProperty("interfaces")]
        public object Interfaces { get; set; }
    }
}