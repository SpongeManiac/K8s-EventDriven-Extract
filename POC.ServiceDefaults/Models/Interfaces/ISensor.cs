namespace POC.ServiceDefaults.Models.Interfaces
{
    public enum SensorType
    {
        Unknown = 0,        // Unknown
        Thermometer = 1,    // Temperature
        Barometer = 2,      // Pressure
        Anemometer = 3,     // Wind Speed
        Hygrometer = 4,     // Humidity
        Pyranometer = 5,    // Solar Irridance
    }
    public interface ISensor
    {
        public int SensorID { get; set; }
        public int DeviceID { get; set; }
        public SensorType  SensorType { get; set; }
    }
}