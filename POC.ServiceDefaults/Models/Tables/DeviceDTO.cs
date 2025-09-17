using Supabase.Postgrest.Models;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;

namespace POC.ServiceDefaults.Models.Tables
{
    [Supabase.Postgrest.Attributes.Table("devices")]
    [System.ComponentModel.DataAnnotations.Schema.Table("devices")]
    public class DeviceDTO: BaseModel
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int DeviceID { get; set; }
        public string? DeviceName { get; set; }
        public string DeviceGPS { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<SensorDTO> Sensors { get; set; } = new List<SensorDTO>();

        [JsonIgnore]
        public Point Point
        {
            get
            {
                var toParse= DeviceGPS.Substring(6, DeviceGPS.Length - 2);
                var splitCoords = toParse.Split(' ');
                double longitude = double.Parse(splitCoords[0]);
                double latitude = double.Parse(splitCoords[1]);
                return new Point(longitude, latitude);
            }
        }
    }
}
