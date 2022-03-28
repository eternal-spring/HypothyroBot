using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class ButtonModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("payload")]
        public object Payload { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("hide")]
        public bool Hide { get; set; }
        public ButtonModel()
        {
            Payload = new object();
        }

        public ButtonModel(string title)
            : this(title, false, null, null)
        {
        }

        public ButtonModel(string title, bool hide)
            : this(title, hide, null, null)
        {
        }

        public ButtonModel(string title, bool hide, object payload)
            : this(title, hide, payload, null)
        {
        }

        public ButtonModel(string title, bool hide, object payload, string url)
        {
            Title = title;
            Hide = hide;
            Payload = payload;
            Url = url;
        }
    }
}