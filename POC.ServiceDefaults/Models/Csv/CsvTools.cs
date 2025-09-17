using CsvHelper;
using CsvHelper.Configuration;
using POC.ServiceDefaults.Models.Csv.ClassMaps;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceDefaults.Models.Csv
{
    public class CsvTools
    {
        public static async Task<Exception?> DataSetToZip(List<IDataSet> datasets, string zipPath)
        {
            try
            {
                using (var zipStream = new FileStream(zipPath, FileMode.Create))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    foreach (var dataset in datasets)
                    {
                        var entry = archive.CreateEntry(dataset.FileName);
                        using (var entryStream = entry.Open())
                        using (var writer = new StreamWriter(entryStream))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            // Necessary to dynamically construct JsonIgnoreClassMap<T> since it has a generic type parameter.
                            // Dataset type is known at runtime, making this possible
                            //Type datasetType = dataset.GetType();
                            //var genericMethod = typeof(CsvContext).GetMethod("RegisterClassMap");
                            //var constructedGenericMethod = genericMethod!.MakeGenericMethod(datasetType);
                            //constructedGenericMethod.Invoke(csv.Context, []);
                            //
                            // Turns out I didnt need the dynamically constructed generic method, but this is still cool to know

                            // Register ClassMaps
                            csv.Context.RegisterClassMap<DeviceMap>();
                            csv.Context.RegisterClassMap<SensorMap>();
                            csv.Context.RegisterClassMap<MeasurementMap>();
                            await csv.WriteRecordsAsync(dataset.GetData());
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
