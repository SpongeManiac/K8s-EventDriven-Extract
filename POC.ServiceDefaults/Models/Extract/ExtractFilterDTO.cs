using POC.ServiceDefaults.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Extract
{
    public class ExtractFilterDTO : IExtractFilter
    {
        public virtual FilterType FilterType { get; set; }
    }
}
