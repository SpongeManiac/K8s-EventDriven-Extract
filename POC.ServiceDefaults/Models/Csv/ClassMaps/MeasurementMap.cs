using CsvHelper.Configuration;
using POC.ServiceDefaults.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Csv.ClassMaps
{
    public class MeasurementMap : ClassMap<MeasurementDTO>
    {
        public MeasurementMap() {
            Map(m => m.MeasurementID);
            Map(m => m.SensorID);
            Map(m => m.Measurement);
            Map(m => m.MeasuredAt);
        }
    }
}
