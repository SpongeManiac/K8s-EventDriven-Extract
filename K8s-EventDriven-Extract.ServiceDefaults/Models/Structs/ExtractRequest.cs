using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Structs
{
    

    // User can request 3 different kinds of extracts:
    // Full Extract - Pulls Devices, Sensors, & Measurements
    //
    // Extract by DeviceID - Pulls devices, sensors, & measurements
    //      - List of DeviceIDs
    //
    // Extract by SensorID - Pulls sensors and their measurements
    //      - List of SensorIDs
    // 
    // Extract by SensorType - Pulls measurements for all sensors of a given type
    //      - SensorType
    public enum ExtractType
    {
        Full = 0,
        ByDeviceID = 1,
        BySensorID = 2,
        BySensorType = 3,
    }
    public class ExtractRequest
    {
        public ExtractType ExtractType { get; set; }
        public List<IExtractFilter> filters { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
