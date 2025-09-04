using Bogus;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Bogus
{
    public class SensorFaker
    {
        public static Faker<SensorDTO> GetFaker(int deviceID)
        {
            SensorType[] sensorTypes = { SensorType.Thermometer, SensorType.Barometer, SensorType.Anemometer, SensorType.Hygrometer, SensorType.Pyranometer };
            return new Faker<SensorDTO>()
                .RuleFor(s => s.SensorID, f => f.IndexFaker + 1)
                .RuleFor(s => s.DeviceID, f => deviceID)
                .RuleFor(s => s.SensorType, f => f.PickRandom(sensorTypes));
        }
    }
}
