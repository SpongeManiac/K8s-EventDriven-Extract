namespace POC.ServiceDefaults.Models.Interfaces
{
    public interface IMeasurement
    {
        public int MeasurementID { get; set; }
        public int SensorID { get; set; }
        public double Measurement { get; set; }
        public DateTimeOffset MeasuredAt { get; set; }

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
