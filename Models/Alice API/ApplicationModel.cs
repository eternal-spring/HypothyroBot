using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class ApplicationModel
    {
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }
    }
}