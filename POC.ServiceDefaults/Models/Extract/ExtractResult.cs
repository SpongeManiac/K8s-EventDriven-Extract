using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Extract
{
    public class ExtractResult
    {
        public string? ExtractPath {  get; set; }
        public Exception? Exception { get; set; }
    }
}
