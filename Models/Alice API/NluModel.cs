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
        public override IEnumerable<EntityModel> ReadJson(JsonReader reader, Type objectType, IEnumerable<EntityModel> existingValue, bool hasExistingValue, JsonSerializer serializer)
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
                        //return JsonConvert.DeserializeObject<NumberModel>((string)jo);
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

//    internal class EntityModelConverter: JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return (objectType == typeof(EntityModel));
//        }

//        //public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
//        //{
//        //    writer.WriteValue(value.ToString());
//        //}

//        //public override IEnumerable<EntityModel> ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
//        //{
//        //    string s = (string)reader.Value;

//        //}
//        //public static EntityModel ToItem(ref JsonReader reader, JsonSerializer options)
//        //{
//        //    if (reader.TokenType == JsonToken.Null)
//        //    {
//        //        return null;
//        //    }

//        //    var readerAtStart = reader;

//        //    string type = null;
//        //    var jsonDocument = JObject.Parse((string)reader.Value);
//        //    if (!string.IsNullOrEmpty(type))
//        //    {
//        //        if (type == "YANDEX.DATETIME")
//        //        {
//        //            return JsonConvert.DeserializeObject<DateTimeModel>(reader)
//        //        }

//        //    }

//        //    throw new NotSupportedException($"{type ?? "<unknown>"} can not be deserialized");
//        //}


//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            JObject jo = JObject.Load(reader);
//            if (jo["type"].Value<string>() == "YANDEX.DATETIME")
//            {
//                //return JsonConvert.DeserializeObject<DateTimeModel>((string)jo);
//                return jo.ToObject<DateTimeModel>(serializer);
//            }
//            if (jo["type"].Value<string>() == "YANDEX.NUMBER")
//            {
//                //return JsonConvert.DeserializeObject<NumberModel>((string)jo);
//                return jo.ToObject<NumberModel>(serializer);
//            }
//            return null;
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}