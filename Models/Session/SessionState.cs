using HypothyroBot.Models.Alice_API;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HypothyroBot.Models.Session
{
    public class SessionState
    {
        [JsonProperty("is_authorised")]
        public bool Authorised { get; set; } = false;
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("last_response")]
        public string LastResponse { get; set; } = "";
        [JsonProperty("last_buttons")]
        public List<ButtonModel> LastButtons { get; set; }
        public SessionState(string lastQuestion)
        {
            LastResponse = lastQuestion;
        }
        public SessionState()
        {

        }
    }
}