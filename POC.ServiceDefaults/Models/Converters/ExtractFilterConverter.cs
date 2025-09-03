

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.ServiceDefaults.Models.Extract;
using POC.ServiceDefaults.Models.Extract.Filters;
using POC.ServiceDefaults.Models.Interfaces;

namespace POC.ServiceDefaults.Models.Converters
{
    public class ExtractFilterConverter : JsonConverter<IExtractFilter>
    {

        public override IExtractFilter ReadJson(JsonReader reader, Type objectType, IExtractFilter? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from reader
            var jObject = JObject.Load(reader);

            // Create converter
            ExtractFilterTypeConverter converter = new ExtractFilterTypeConverter();

            // Peek at filter type prop
            var filterTypeToken = jObject["FilterType"]
            ?? throw new JsonSerializationException("FilterType property is missing");

            //FilterType? filterType = converter.ReadJson(filterTypeToken.CreateReader(), typeof(FilterType), FilterType.DateTimeRange, false, serializer);
            FilterType filterType = serializer.Deserialize<FilterType>(filterTypeToken.CreateReader());

            // Deserialize into correct concrete type
            switch(filterType)
            {
                case FilterType.Geolocation:
                    return jObject.ToObject<GeolocationFilter>(serializer)!;
                case FilterType.DateTimeRange:
                    return jObject.ToObject<DateTimeRangeFilter>(serializer)!;
                default:
                    throw new JsonSerializationException($"Unknown FilterType: {filterType}");
            }
        }

        public override void WriteJson(JsonWriter writer, IExtractFilter? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
