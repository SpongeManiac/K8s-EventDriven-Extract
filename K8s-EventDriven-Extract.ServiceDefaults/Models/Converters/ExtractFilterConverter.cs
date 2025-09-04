using POC.ServiceDefaults.Models.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Converters
{
    public class ExtractFilterConverter : JsonConverter<IExtractFilter>
    {
        public override IExtractFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("filterType", out var typeProp))
                throw new JsonException("Missing filterType");

            var type = (FilterType)typeProp.GetInt32();

            return type switch
            {
                FilterType.Geolocation => JsonSerializer.Deserialize<GeolocationFilter>(root.GetRawText(), options),
                FilterType.DateTimeRange => JsonSerializer.Deserialize<DateTimeRangeFilter>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown filter type: {type}")
            };
        }

        public override void Write(Utf8JsonWriter writer, IExtractFilter value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

}
