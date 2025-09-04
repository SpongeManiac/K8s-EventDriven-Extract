using Bogus;
using POC.ServiceDefaults.Models.Structs;
using POC.ServiceDefaults.Models.Tables;

namespace POC.ServiceDefaults.Models.Bogus
{
    public class DeviceFaker
    {
        public static Faker<DeviceDTO> GetFaker()
        {
            return new Faker<DeviceDTO>()
                .RuleFor(u => u.DeviceID, f => f.IndexFaker + 1)
                .RuleFor(u => u.DeviceName, f => f.Hacker.IngVerb() + f.Hacker.Noun())
                .RuleFor(
                    u => u.DeviceGPS, 
                    f => {
                        return new GPSCoordinates() { Latitude = f.Address.Latitude(), Longitude = f.Address.Longitude() };
                    }
                 )    
                .RuleFor(u => u.CreatedAt, f => f.Date.PastOffset(yearsToGoBack: 3, refDate: new DateTimeOffset(new DateTime(year: 2024, month: 1, day: 1))).DateTime);
        }
    }
}
