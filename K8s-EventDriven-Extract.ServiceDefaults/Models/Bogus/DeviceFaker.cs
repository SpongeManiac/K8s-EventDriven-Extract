using Bogus;
using POC.ServiceDefaults.Models.DataStructures;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Bogus
{
    public class DeviceFaker
    {
        public static Faker<Device> GetFaker()
        {
            return new Faker<Device>()
                .RuleFor(u => u.DeviceID, f => f.IndexFaker + 1)
                .RuleFor(u => u.DeviceName, f => f.Hacker.IngVerb() + f.Hacker.Noun())
                .RuleFor(u => u.DeviceGPS, f => new GPSCoordinates() { Latitude = f.Address.Latitude(), Longitude = f.Address.Longitude() })
                .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset(yearsToGoBack: 3, refDate: new DateTimeOffset(new DateTime(year: 2024, month: 1, day: 1))).DateTime);
        }
    }
}
