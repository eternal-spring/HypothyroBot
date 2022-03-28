using Newtonsoft.Json;
using HypothyroBot.Models.Session;
using System.Collections.Generic;

namespace HypothyroBot.Models.Alice_API
{
    public class AliceResponse
    {
        [JsonProperty("response")]
        public ResponseModel Response { get; set; }
        [JsonProperty("user_state_update")]
        public SessionState SessionState { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";
        public AliceResponse() 
        {

        }
        public AliceResponse(AliceRequest aliceRequest, string text, string tts, bool endSession, List<ButtonModel> buttons)
        {
            Response = new ResponseModel()
            {
                Text = text,
                Tts = tts ?? text,
                Buttons = buttons,
                EndSession = endSession,
            };
            if (aliceRequest.State != null)
            {
                SessionState = aliceRequest.State.Session;
            }
        }
        public AliceResponse(AliceRequest aliceRequest, string text, string tts, List<ButtonModel> buttons) : this(aliceRequest, text, tts, false, buttons)
        {
        }
        public AliceResponse(AliceRequest aliceRequest, string text) : this(aliceRequest, text, text) 
        { 
        }
        public AliceResponse(AliceRequest aliceRequest, string text, bool endSession) : this(aliceRequest, text, text, endSession, new List<ButtonModel>())
        {
        }
        public AliceResponse(AliceRequest aliceRequest, string text, string tts) :this(aliceRequest,text,tts, new List<ButtonModel>())
        {
        }
        public AliceResponse(AliceRequest aliceRequest, string text, List<ButtonModel> buttons) : this(aliceRequest, text, text, buttons)
        {
        }
    }
}