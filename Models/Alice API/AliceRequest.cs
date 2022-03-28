using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class AliceRequest
    {
        [JsonProperty("meta")]
        public MetaModel Meta { get; set; }
        [JsonProperty("request")]
        public RequestModel Request { get; set; }
        [JsonProperty("session")]
        public SessionModel Session { get; set; }
        [JsonProperty("state")]
        public StateModel State { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}