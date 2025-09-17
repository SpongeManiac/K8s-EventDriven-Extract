using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.ServiceDefaults.Models.Extract;
using POC.ServiceDefaults.Models.Extract.Filters;
using POC.ServiceDefaults.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Converters
{
    public class ExtractFilterListConverter : JsonConverter<List<IExtractFilter>>
    {
        private readonly ExtractFilterConverter _itemConverter = new();

        public override List<IExtractFilter> ReadJson(JsonReader reader, Type objectType, List<IExtractFilter> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            var list = new List<IExtractFilter>();

            foreach (var token in array)
            {
                //var item = _itemConverter.ReadJson(token.CreateReader(), typeof(ExtractFilterDTO), null, false, serializer);
                // Peek at filter type prop
                var filterTypeToken = token["FilterType"]
                ?? throw new JsonSerializationException("FilterType property is missing");

                //FilterType? filterType = converter.ReadJson(filterTypeToken.CreateReader(), typeof(FilterType), FilterType.DateTimeRange, false, serializer);
                FilterType filterType = serializer.Deserialize<FilterType>(filterTypeToken.CreateReader());

                // Deserialize into correct concrete type
                switch (filterType)
                {
                    case FilterType.Geolocation:
                        list.Add(token.ToObject<GeolocationFilter>(serializer)!);
                        break;
                    case FilterType.DateTimeRange:
                        list.Add(token.ToObject<DateTimeRangeFilter>(serializer)!);
                        break;
                    default:
                        throw new JsonSerializationException($"Unknown FilterType: {filterType}");
                }
            }

            return list;
        }

        public override void WriteJson(JsonWriter writer, List<IExtractFilter> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                _itemConverter.WriteJson(writer, item, serializer);
            }
            writer.WriteEndArray();
        }
    }
}
