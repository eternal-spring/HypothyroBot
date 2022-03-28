using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HypothyroBot.Models.Alice_API
{
    public class NluModel
    {
        [JsonProperty("tokens")]
        public IEnumerable<string> Tokens { get; set; }
        [JsonProperty("entities")]
        [JsonConverter(typeof(EntitiesConverter))]
        public IEnumerable<EntityModel> Entities { get; set; }
    }

    internal class EntitiesConverter : JsonConverter<IEnumerable<EntityModel>>
    {
        public override IEnumerable<EntityModel> ReadJson(JsonReader reader, Type objectType, 
            IEnumerable<EntityModel> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var obj = JArray.Load(reader);
                var list = new List<EntityModel>();
                foreach (JObject ent in obj.Children())
                {
                    if (ent["type"].Value<string>() == "YANDEX.DATETIME")
                    {
                        list.Add(ent.ToObject<DateTimeModel>(serializer));
                    }
                    else if (ent["type"].Value<string>() == "YANDEX.NUMBER")
                    {
                        list.Add(ent.ToObject<NumberModel>(serializer));
                    }
                }
                return list.ToArray();
            }
            throw new NotImplementedException();
        }
        public override void WriteJson(JsonWriter writer, IEnumerable<EntityModel> value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}