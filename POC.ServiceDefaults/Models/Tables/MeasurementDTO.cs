using Newtonsoft.Json;
using POC.ServiceDefaults.Models.Interfaces;
using Supabase.Postgrest.Models;

namespace POC.ServiceDefaults.Models.Tables
{
    [Supabase.Postgrest.Attributes.Table("measurements")]
    [System.ComponentModel.DataAnnotations.Schema.Table("measurements")]

    public class MeasurementDTO : BaseModel, IMeasurement
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int MeasurementID { get; set; }
        public int SensorID { get; set; }

        // Navigation property for EFCore
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(SensorID))]
        [JsonIgnore]
        public SensorDTO Sensor { get; set; } = null!;

        public double Measurement { get; set; }
        public DateTimeOffset MeasuredAt { get; set; }
    }
}
