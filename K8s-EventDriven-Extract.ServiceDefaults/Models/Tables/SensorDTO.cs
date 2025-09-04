using POC.ServiceDefaults.Models.Interfaces;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDefaults.Models.Tables
{
    public class SensorDTO: BaseModel, ISensor
    {
        public int SensorId { get; set; }
        public int SensorID { get; set; }
        public int DeviceID { get; set; }
        public SensorType SensorType { get; set; }

    }
}
