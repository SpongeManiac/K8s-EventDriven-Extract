using Bogus;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Models.Bogus
{
    public enum Coverage
    {
        Clear,          // No clouds
        MostlyClear,    // A few clouds
        PartlyCloudy,   // Moderate cloud cover
        MostlyCloudy,   // Significant cloud cover
        Overcast        // Fully covered sky
    }

    public enum Rainfall
    {
        None,           // No rain
        Light,          // Drizzle or light showers
        Moderate,       // Regular rain, not heavy
        Heavy,          // Strong rain
        Torrential      // Very intense rain or downpour
    }

    public enum Temperature
    {
        Freezing,       // Below 0°C / 32°F
        Cold,           // 0°C–10°C / 32°F–50°F
        Cool,           // 11°C–20°C / 51°F–68°F
        Warm,           // 21°C–30°C / 69°F–86°F
        Hot,            // 31°C–40°C / 87°F–104°F
        Scorching       // Above 40°C / 104°F
    }

    class Weather
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public double windSpeed { get; set; }
        public double humidity { get; set; }
        public double luminence { get; set; }
        public double rainDepth { get; set; }
    }

    public class MeasurementFaker : Faker<MeasurementDTO>
    {
        public int sensorID;
        public SensorType sensorType;
        public DateTime deviceCreation;
        public MeasurementFaker(
            int sensorID,
            SensorType sensorType,
            DateTime deviceCreation
        )
        {
            this.sensorID = sensorID;
            this.sensorType = sensorType;
            this.deviceCreation = deviceCreation;

            RuleFor(m => m.MeasurementID, f => f.IndexFaker + 1);
            RuleFor(m => m.SensorID, f => this.sensorID);
            RuleFor(m => m.MeasuredAt, f => f.Date.BetweenDateOnly(DateOnly.FromDateTime(this.deviceCreation), DateOnly.FromDateTime(DateTime.Now)).ToDateTime(new TimeOnly(12)));
            RuleFor(m => m.Measurement, f =>
            {
                switch (sensorType)
                {
                    case SensorType.Thermometer:
                        return f.Random.Double(-5, 80);
                    case SensorType.Barometer:
                        return f.Random.Double(870, 1050);
                    case SensorType.Anemometer:
                        return f.Random.Double(0, 20);
                    case SensorType.Hygrometer:
                        return f.Random.Double(0, 100);
                    case SensorType.Pyranometer:
                        return f.Random.Double(100, 1500);
                    default:
                        throw new NotImplementedException();
                }
            });

        }
    }
}