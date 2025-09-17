using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POC.ServiceDefaults.Models.Context;
using POC.ServiceDefaults.Models.Csv;
using POC.ServiceDefaults.Models.Extract;
using POC.ServiceDefaults.Models.Extract.Filters;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using Supabase;

namespace MQConsume
{
    public class CreateExtract
    {
        private readonly ILogger _logger;
        private readonly Client _supabase;
        private readonly SupabaseContext _supadb;

        public CreateExtract(
            ILoggerFactory loggerFactory,
            Client supabase,
            SupabaseContext supadb
        )
        {
            _logger = loggerFactory.CreateLogger<CreateExtract>();
            _supabase = supabase;
            _supadb = supadb;
        }

        [Function("CreateExtract")]
        public async Task Run([RabbitMQTrigger("create_extract", ConnectionStringSetting = "rabbitmq")] string requestJson)
        {
            // Parse object
            try
            {
                var extractRequest = JsonConvert.DeserializeObject<ExtractRequest>(requestJson);
                if (extractRequest == null)
                {
                    throw new JsonException($"Parsed null object from JSON:\n{requestJson}");
                }
                ExtractResult results = null!;
                switch (extractRequest.ExtractType)
                {
                    case ExtractType.Full:
                        results = await FullExtract(extractRequest.RequestedBy, extractRequest.Filters);
                        
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (results.ExtractPath == null)
                {
                    throw results.Exception!;
                }

                _logger.LogInformation($"Extract success! Path: {results.ExtractPath}");
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"FAIL: Could not generate extract from request.\nRequest Json:\n{requestJson}");
            }
            
        }

        private async Task<Tuple<List<IDataSet>, Exception?>> GetFullExtractData(List<IExtractFilter> filters)
        {
            List<DeviceDTO> devices = new List<DeviceDTO>();
            string devicesCsv = "devices.csv";
            string sensorsCsv = "sensors.csv";
            string measurementsCsv = "measurements.csv";
            var datasets = new List<IDataSet>();
            try
            {
                var deviceSensorQuery = _supadb.Devices.Include(d => d.Sensors);
                List<GeolocationFilter> geoFilters = filters.FindAll(f => f.FilterType == FilterType.Geolocation).Cast<GeolocationFilter>().ToList();
                List<DateTimeRangeFilter> rangeFilters = filters.FindAll(f => f.FilterType == FilterType.DateTimeRange).Cast<DateTimeRangeFilter>().ToList();

                GeolocationFilter geoFilter = null!;
                NetTopologySuite.Geometries.Point refPoint = NetTopologySuite.Geometries.Point.Empty;
                DateTimeRangeFilter rangeFilter = null!;


                bool hasGeoFilter = geoFilters.Count > 0;
                bool hasRangeFilter = rangeFilters.Count > 0;
                if (hasGeoFilter)
                {
                    geoFilter = geoFilters.First();
                    refPoint = new NetTopologySuite.Geometries.Point(
                        geoFilter.GPSCoordinates.Longitude,
                        geoFilter.GPSCoordinates.Latitude
                    );
                }
                if (hasRangeFilter) rangeFilter = rangeFilters.First();

                if (hasGeoFilter && hasRangeFilter)
                {
                    // Both geo and range filter. Get devices, sensors, and measurements that are within a given geolocation and date/time range
                    devices = await deviceSensorQuery.ThenInclude(
                        s => s.Measurements.Where(
                            m => m.MeasuredAt >= rangeFilter.Start
                            && m.MeasuredAt <= rangeFilter.End
                        )
                    ).Where(
                        d => d.Point.IsWithinDistance(refPoint, geoFilter.Range)
                    ).ToListAsync();
                }
                else if (hasGeoFilter && !hasRangeFilter)
                {
                    // Just geo filter. Grab devices that are within the given distance to a reference point.
                    devices = await deviceSensorQuery.ThenInclude(s => s.Measurements).Where(
                        d => d.Point.IsWithinDistance(refPoint, geoFilter.Range)
                    ).ToListAsync();
                }
                else if (!hasGeoFilter && hasRangeFilter)
                {
                    // Just date range filter. Grab devices that have a measurement in the datetime range
                    devices = await deviceSensorQuery.ThenInclude(
                        s => s.Measurements.Where(
                            m => m.MeasuredAt >= rangeFilter.Start
                            && m.MeasuredAt <= rangeFilter.End
                        )
                    ).ToListAsync();
                }
                else
                {
                    // No filters, grab EVERYTHING
                    devices = await deviceSensorQuery.ThenInclude(s => s.Measurements).ToListAsync();
                }
                var sensors = devices.SelectMany(d => d.Sensors).ToList();
                datasets.Add(new DataSet<DeviceDTO>() { FileName = devicesCsv, Data = devices});
                datasets.Add(new DataSet<SensorDTO>() { FileName = sensorsCsv, Data = sensors});
                datasets.Add(new DataSet<MeasurementDTO>() { FileName = measurementsCsv, Data = sensors.SelectMany(s => s.Measurements).ToList()});
                return new Tuple<List<IDataSet>, Exception?>(datasets, null);
            }
            catch (Exception ex)
            {
                return new Tuple<List<IDataSet>, Exception?>(datasets, ex);
            }
        }

        private async Task<ExtractResult> FullExtract(string requestedBy, List<IExtractFilter> filters) 
        {
            string zipName = $"FullExtract_{requestedBy}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip";
            string fullPath = $"/tmp/{zipName}";
            try
            {
                // Get filtered datasets
                var datasetsResult = await GetFullExtractData(filters);
                if (datasetsResult.Item2 != null) throw datasetsResult.Item2;

                // Create ZIP
                var exception = await CsvTools.DataSetToZip(datasetsResult.Item1, fullPath);
                if (exception != null) throw exception;
                // Upload ZIP
                await _supabase.Storage.From("extracts").Upload(
                    fullPath,
                    zipName,
                    new Supabase.Storage.FileOptions() { 
                        CacheControl = "3600",
                        Upsert = false,
                        ContentType = "application/zip"
                    }
                );
                return new ExtractResult()
                {
                    ExtractPath = zipName,
                };
            }
            catch (Exception ex)
            {
                // Could not perform extract for some reason
                return new ExtractResult()
                {
                    Exception = ex
                };
            }
        }
    }
}
