namespace HypothyroBot.Models.Session
{
    using HypothyroBot.Models.Alice_API;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class SessionState
    {
        //[JsonProperty("mode")]
        //public ModeType Mode { get; set; }
        [JsonProperty("is_authorised")]
        public bool Authorised { get; set; } = false;
        [JsonProperty("last_response")]
        public string LastResponse { get; set; } = "";
        [JsonProperty("last_buttons")]
        public List<ButtonModel> LastButtons { get; set; }
        public SessionState(string lastQuestion)
        {
            //Mode = mode;
            LastResponse = lastQuestion;
        }
        public SessionState()
        {
        }
    }
}