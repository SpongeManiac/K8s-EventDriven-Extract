using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Interfaces
{
    public interface ISensorMeasurement
    {
        public int MeasurementID { get; set; }
        public int SensorID { get; set; }
        public double Measurement { get; set; }
        public DateTime MeasuredAt { get; set; }

        public static string DisplayUnit(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.Thermometer:
                    return "°C";
                case SensorType.Barometer:
                    return "hPa";
                case SensorType.Anemometer:
                    return "m/s";
                case SensorType.Hygrometer:
                    return "%";
                case SensorType.Pyranometer:
                    return "W/m²";
                default:
                    return String.Empty;
            }
        }
    }
}
