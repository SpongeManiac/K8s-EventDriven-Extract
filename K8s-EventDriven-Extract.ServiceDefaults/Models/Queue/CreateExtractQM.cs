using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Queue
{
    public enum ExtractType
    {
        Weather,
    }

    public class CreateExtractQM
    {
        public ExtractType ExtractType { get; set; }
        public DateTime DateRequested { get; set; }
        public string RequestedBy { get; set; }
        public string? Json { get; set; }
    }
}
