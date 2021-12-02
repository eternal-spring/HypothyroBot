using HypothyroBot.Models.Session;
using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class StateModel
    {
        [JsonProperty("user")]
        public SessionState Session { get; set; }

        //[JsonProperty("user")]
        //public UserState User { get; set; }
    }
}