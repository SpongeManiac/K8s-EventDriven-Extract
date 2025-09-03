using Bogus;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Bogus
{
    public class SensorFaker
    {
        public static Faker<ISensor> GetFaker(int deviceID)
        {
            var sensorTypes = Enum.GetValues(typeof(SensorType)).Cast<SensorType>();
            return new Faker<ISensor>()
                .RuleFor(s => s.SensorID, f => f.IndexFaker + 1)
                .RuleFor(s => s.DeviceID, f => deviceID)
                .RuleFor(s => s.SensorType, f => f.PickRandom(sensorTypes));
        }
    }
}
