using POC.ServiceDefaults.Models.Extract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Interfaces
{
    public enum FilterType
    {
        Geolocation = 0,
        DateTimeRange = 1,
    }
    public interface IExtractFilter
    {
        FilterType FilterType { get; }
    }
}
