using POC.ServiceDefaults.Models.Interfaces;
using Supabase.Postgrest.Models;
using POC.ServiceDefaults.Models.Converters;
using Newtonsoft.Json;

namespace POC.ServiceDefaults.Models.Tables
{
    [Supabase.Postgrest.Attributes.Table("sensors")]
    [System.ComponentModel.DataAnnotations.Schema.Table("sensors")]
    public class SensorDTO: BaseModel, ISensor
    {

        [System.ComponentModel.DataAnnotations.Key]
        public int SensorID { get; set; }
        public int DeviceID { get; set; }

        [JsonConverter(typeof(SensorTypeConverter))]
        public SensorType SensorType { get; set; }

        // Navigation property for EFCore
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(DeviceID))]
        [JsonIgnore]
        public DeviceDTO Device { get; set; } = null!;

        // Navigation property for EFCore
        [JsonIgnore]
        public ICollection<MeasurementDTO> Measurements { get; set; } = new List<MeasurementDTO>();
    }
}
