using CsvHelper.Configuration;
using Newtonsoft.Json;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Csv.ClassMaps
{
    public class SensorMap : ClassMap<SensorDTO>
    {
        public SensorMap() {
            Map(s => s.SensorID);
            Map(s => s.DeviceID);
            Map(s => s.SensorType).Convert(s => ((int)s.Value.SensorType).ToString());
        }
    }
}
