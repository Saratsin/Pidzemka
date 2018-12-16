using System;
using Newtonsoft.Json;
using Pidzemka.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Pidzemka.JsonConverters
{
    public class MapJsonConverter : JsonConverter<Map>
    {
        public override Map ReadJson(JsonReader reader, Type objectType, Map existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObj = JObject.Load(reader);

            var defaultFromStationId = jsonObj["DefaultFromStationId"].ToObject<int>(serializer);
            var stations = jsonObj["Stations"].ToObject<Dictionary<int, Station>>(serializer);
            var lineParts = jsonObj["LineParts"].ToObject<List<LinePart>>(serializer);

            return new Map(defaultFromStationId, stations, lineParts);
        }

        public override void WriteJson(JsonWriter writer, Map value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
