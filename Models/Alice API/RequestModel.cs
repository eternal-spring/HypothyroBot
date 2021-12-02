using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class RequestModel
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("original_utterance")]
        public string OriginalUtterance { get; set; }

        //[JsonProperty("payload")]
        //public object Payload { get; set; }

        [JsonProperty("markup")]
        public MarkupModel Markup { get; set; }

        [JsonProperty("type")]
        public RequestType Type { get; set; }
        [JsonProperty("nlu")]
        public NluModel Nlu { get; set; }
    }
    public enum RequestType
    {
        SimpleUtterance,
        ButtonPressed
    }

}