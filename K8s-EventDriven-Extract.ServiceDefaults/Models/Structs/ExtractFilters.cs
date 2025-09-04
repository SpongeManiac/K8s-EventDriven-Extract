using POC.ServiceDefaults.Models.Structs;

namespace POC.ServiceDefaults.Models.Structs
{
    // User can apply pre-filters
    // 1. Filter by Geolocation (Optional)
    //      - Longitude
    //      - Latitude
    //      - Max Distance
    //
    // 2. Filter by DateTime range (Optional)
    //      - DateTime range
    public enum FilterType
    {
        Geolocation = 0,
        DateTimeRange = 1,
    }

    public interface IExtractFilter
    {
        FilterType FilterType { get; }
    }

    public class GeolocationFilter : IExtractFilter
    {
        public FilterType FilterType { get; set; } = FilterType.Geolocation;
        public GPSCoordinates GPSCoordinates { get; set; }
        public double Range { get; set; }
    }

    public class DateTimeRangeFilter : IExtractFilter
    {
        public FilterType FilterType { get; set; } = FilterType.DateTimeRange;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}