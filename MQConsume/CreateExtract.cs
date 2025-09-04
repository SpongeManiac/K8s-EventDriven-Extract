using System;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using POC.ServiceDefaults.Models.Converters;
using POC.ServiceDefaults.Models.Structs;
using POC.ServiceDefaults.Models.Tables;
using ServiceDefaults.Models.Tables;
using Supabase;
using Supabase.Postgrest.Interfaces;

namespace MQConsume
{
    public class CreateExtract
    {
        private readonly ILogger _logger;
        private readonly Client _supabase;

        public CreateExtract(ILoggerFactory loggerFactory, Client supabase)
        {
            _logger = loggerFactory.CreateLogger<CreateExtract>();
            _supabase = supabase;
        }

        [Function("CreateExtract")]
        public async Task Run([RabbitMQTrigger("create_extract", ConnectionStringSetting = "rabbitmq")] string requestJson)
        {
            // Add custom converters
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new ExtractFilterConverter());
            // Parse object
            try
            {
                var extractRequest = JsonSerializer.Deserialize<ExtractRequest>(requestJson, options);
                if (extractRequest == null)
                {
                    throw new JsonException($"Parsed null object from JSON:\n{requestJson}");
                }

                // Extract data from DB based on extract
                _logger.LogInformation($"An extract has been requested:\nType: {extractRequest.ExtractType}\nDate: {extractRequest.RequestedAt}\nRequested By: {extractRequest.RequestedBy}");

                // Create inital stub
                IPostgrestTable<DeviceDTO> deviceResults = _supabase.From<DeviceDTO>();
                IPostgrestTable<SensorDTO> sensorResults = _supabase.From<SensorDTO>();
                IPostgrestTable<MeasurementDTO> measurementResults = _supabase.From<MeasurementDTO>();

                // Change query based on filters and extract type
                switch(extractRequest.ExtractType)
                {
                    case ExtractType.Full:
                        break;

                }

                // Add filters as necessary
                foreach (var filter in extractRequest.filters) {
                    switch (filter.FilterType) {
                        case FilterType.Geolocation:
                            // ST_DWithin ( Geograph1, Geograph2, Distance ) // PostGIS function to filter based on distance
                            // ST_SetSRID( Geography, Spatial Reference ID ) // PostGIS function to convert coordinates to a specific system (4326 is standard GPS)
                            // ::geography ensures the type is casted to a geography type
                            // @ is for substituation
                            deviceResults.Filter()
                            deviceResults.Filter("ST_DWithin( DeviceGPS, ST_SetSRID(ST_POINT(@long, @lat), 4326)::geography, @radius)", new Dictionary<string, object> { 
                                
                            });
                            break;
                    }
                }

                var result = .Filter("CreatedAt", Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual, DateTime.Now);
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"FAIL: Could not parse queue object from JSON:\n{requestJson}");
            }
            
        }
    }
}
