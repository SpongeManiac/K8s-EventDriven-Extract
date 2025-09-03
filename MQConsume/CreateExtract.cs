using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using POC.ServiceDefaults.Models.Queue;
using POC.ServiceDefaults.Models.Tables;
using Supabase;

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
        public async Task Run([RabbitMQTrigger("create_extract", ConnectionStringSetting = "rabbitmq")] CreateExtractQM extractRequest)
        {
            
            _logger.LogInformation($"An extract has been requested:\nType: {extractRequest.ExtractType}\nDate: {extractRequest.DateRequested}\nRequested By: {extractRequest.RequestedBy}");
            _logger.LogInformation($"Supabase Connected: ");
        }
    }
}
