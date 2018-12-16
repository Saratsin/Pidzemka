using System;
using Newtonsoft.Json;
using Pidzemka.Models;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Drawing;

public class StationJsonConverter : JsonConverter<Station>
{
    public override Station ReadJson(JsonReader reader, Type objectType, Station existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObj = JObject.Load(reader);

        var jCoordinates = jsonObj["Coordinates"];
        var coordinates = new PointF(
            jCoordinates["X"].ToObject<float>(serializer), 
            jCoordinates["Y"].ToObject<float>(serializer)
        );

        return new Station(coordinates);
    }

    public override void WriteJson(JsonWriter writer, Station value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}