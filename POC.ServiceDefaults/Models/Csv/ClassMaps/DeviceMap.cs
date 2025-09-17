using CsvHelper.Configuration;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Csv.ClassMaps
{
    public class DeviceMap : ClassMap<DeviceDTO>
    {
        public DeviceMap()
        {
            Map(d => d.DeviceID);
            Map(d => d.DeviceName);
            Map(d => d.DeviceGPS);
            Map(d => d.CreatedAt);
        }
    }
}
