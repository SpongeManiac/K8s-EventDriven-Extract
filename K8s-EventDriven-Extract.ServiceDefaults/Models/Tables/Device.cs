using POC.ServiceDefaults.Models.DataTransferObjects;
using POC.ServiceDefaults.Models.DataStructures;
using POC.ServiceDefaults.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace POC.ServiceDefaults.Models.Tables
{
    [Table("devices")]
    public class Device: BaseModel
    {
        public int DeviceID { get; set; }
        public string? DeviceName { get; set; }
        public GPSCoordinates? DeviceGPS { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
