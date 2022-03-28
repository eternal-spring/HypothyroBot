using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class DateTimeModel: EntityModel
    {
        [JsonProperty("value")]
        public DateValueModel Value { get; set; }
    }
}