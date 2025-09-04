using POC.ServiceDefaults.Models.Interfaces;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Tables
{
    public class MeasurementDTO : BaseModel, IMeasurement
    {
        public int MeasurementID { get; set; }
        public int SensorID { get; set; }
        public double Measurement { get; set; }
        public DateTime MeasuredAt { get; set; }
    }
}
