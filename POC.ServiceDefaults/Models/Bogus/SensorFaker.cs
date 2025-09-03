using Bogus;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using System.Transactions;

namespace POC.ServiceDefaults.Models.Bogus
{
    public class SensorFaker : Faker<SensorDTO>
    {
        int deviceID { get; set; }
        public int currentSensorID { get; set; }
        SensorType[] sensorTypes = { SensorType.Thermometer, SensorType.Barometer, SensorType.Anemometer, SensorType.Hygrometer, SensorType.Pyranometer };

        int IncrementID(Faker faker)
        {
            currentSensorID+=1;
            return currentSensorID;
        }

        public SensorFaker(int deviceID, int startID)
        {
            this.deviceID = deviceID;
            this.currentSensorID = startID;
            RuleFor(s => s.SensorID, IncrementID);
            RuleFor(s => s.DeviceID, f => deviceID);
            RuleFor(s => s.SensorType, f => f.PickRandom(sensorTypes));
        }
    }
}
