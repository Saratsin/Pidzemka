using System;
using Newtonsoft.Json;
using Pidzemka.Models;
using Newtonsoft.Json.Linq;
namespace Pidzemka.JsonConverters
{
    public class LinePartJsonConverter : JsonConverter<LinePart>
    {
        public override LinePart ReadJson(JsonReader reader, Type objectType, LinePart existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObj = JObject.Load(reader);

            var startStationId = jsonObj["StartStationId"].ToObject<int>(serializer);
            var endStationId = jsonObj["EndStationId"].ToObject<int>(serializer);
            var timeDistance = jsonObj["TimeDistance"].ToObject<TimeSpan>(serializer);

            return new LinePart(startStationId, endStationId, timeDistance);
        }

        public override void WriteJson(JsonWriter writer, LinePart value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
