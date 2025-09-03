using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Csv
{
    public interface IDataSet
    {
        public string FileName { get; set; }
        public IEnumerable<object> GetData();
        public List<T> GetData<T>()
        {
            return GetData().Cast<T>().ToList();
        }
    }
    public class DataSet<T>: IDataSet where T : class
    {
        public string FileName { get; set; }
        public List<T> Data { get; set; } = new();
        public IEnumerable<object> GetData() { return Data; }
    }
}
