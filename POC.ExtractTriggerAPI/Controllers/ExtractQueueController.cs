using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using POC.ServiceDefaults.Models.Extract;
using POC.ServiceDefaults.Models.Extract.Filters;
using POC.ServiceDefaults.Models.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace EchoAPI.Controllers
{
    [ApiController]
    [Route("api/extract")]
    public class ExtractQueueController : ControllerBase
    {
        private readonly ILogger<ExtractQueueController> _logger;
        private readonly IConnection _mqConnection;

        public ExtractQueueController(
            ILogger<ExtractQueueController> logger,
            IConnection connection
        )
        {
            _logger = logger;
            _mqConnection = connection;
        }

        [HttpPost(Name = "RequestExtract")]
        [Consumes("application/json")]
        public async Task<IActionResult> Post(
            [FromBody] ExtractRequest extractRequest
        )
        {
            try
            {
                using (IChannel channel = await _mqConnection.CreateChannelAsync())
                {
                    // Send message to queue
                    _logger.LogInformation("");
                    var json = JsonConvert.SerializeObject(extractRequest);
                    _logger.LogInformation($"Request JSON:\n{json}\nChannel object exists: {channel.ChannelNumber}");
                    await channel.BasicPublishAsync("", "create_extract", body: Encoding.UTF8.GetBytes(json));
                }
                return StatusCode(StatusCodes.Status201Created);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }

        [HttpGet(Name = "RequestExtractTest")]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogInformation("Testing extract request!");
                using (IChannel channel = await _mqConnection.CreateChannelAsync())
                {
                    ExtractRequest extractRequest = new ExtractRequest()
                    {
                        ExtractType = ExtractType.Full,
                        Filters = new List<IExtractFilter>(){
                            new DateTimeRangeFilter()
                            {
                                Start = DateTime.Now - TimeSpan.FromDays(365*2),
                                End = DateTime.Now,
                            },
                        },
                        RequestedAt = DateTime.Now,
                        RequestedBy = "John_Doe"
                    };
                    // Send message to queue
                    _logger.LogInformation($"Testing extract request:\n{extractRequest.ExtractType}");
                    string json = JsonConvert.SerializeObject(extractRequest);
                    _logger.LogInformation($"Request JSON:\n{json}\nChannel object exists: {channel.ChannelNumber}");
                    // Send message to queue
                    await channel.BasicPublishAsync("", "create_extract", body: Encoding.UTF8.GetBytes(json));
                }
                return StatusCode(StatusCodes.Status201Created);
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "Could not queue extract request.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
