

using Newtonsoft.Json;
using POC.ServiceDefaults.Models.Extract;
using POC.ServiceDefaults.Models.Interfaces;

namespace POC.ServiceDefaults.Models.Converters
{
    public class ExtractFilterTypeConverter : JsonConverter<FilterType>
    {
        int maxValue = Enum.GetValues(typeof(FilterType)).Cast<int>().Max();

        public override FilterType ReadJson(JsonReader reader, Type objectType, FilterType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                int value = Convert.ToInt32(reader.Value); // 0 if null, which works bc SensorType(0) is unknown.
                value = value < 0 || value > maxValue ? 0 : value;
                return ((FilterType)value);
            }

            if (reader.TokenType == JsonToken.String)
            {
                string str = reader.Value!.ToString()!;
                if (Enum.TryParse<FilterType>(str, true, out var result))
                {
                    return result;
                }

                // Fallback to default if invalid string (default for enums is the 0 value)
                return default;
            }

            throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing SensorType");
        }

        public override void WriteJson(JsonWriter writer, FilterType value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value);
        }
    }
}
