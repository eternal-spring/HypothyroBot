using Newtonsoft.Json;
using System.Collections.Generic;

namespace HypothyroBot.Models.Alice_API
{
    public class ResponseModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("tts")]
        public string Tts { get; set; }
        [JsonProperty("end_session")]
        public bool EndSession { get; set; }
        [JsonProperty("buttons")]
        public List<ButtonModel> Buttons { get; set; }
    }
}