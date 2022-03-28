using Newtonsoft.Json;

namespace HypothyroBot.Models.Alice_API
{
    public class DateValueModel
    {
        [JsonProperty("day")]
        public double Day { get; set; }
        [JsonProperty("day_is_relative")]
        public bool DayIsRelative { get; set; }
        [JsonProperty("month")]
        public double Month { get; set; }
        [JsonProperty("year")]
        public double Year { get; set; }
    }
}