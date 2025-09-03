using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Structs;

namespace POC.ServiceDefaults.Models.Extract.Filters
{
    public class GeolocationFilter : ExtractFilterDTO
    {
        public override FilterType FilterType { get; set; } = FilterType.Geolocation;
        public GPSCoordinates GPSCoordinates { get; set; }
        public double Range { get; set; }
    }
}