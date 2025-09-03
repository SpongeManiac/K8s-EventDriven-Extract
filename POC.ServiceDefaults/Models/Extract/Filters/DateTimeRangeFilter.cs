using POC.ServiceDefaults.Models.Interfaces;

namespace POC.ServiceDefaults.Models.Extract.Filters
{
    public class DateTimeRangeFilter : ExtractFilterDTO, IExtractFilter
    {
        public FilterType FilterType { get; set; } = FilterType.DateTimeRange;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}