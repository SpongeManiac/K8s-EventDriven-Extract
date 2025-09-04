using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceDefaults.Models.Structs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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
                using (var channel = await _mqConnection.CreateChannelAsync())
                {
                    // Send message to queue
                    await channel.BasicPublishAsync(string.Empty, "create_extract", body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(extractRequest)));
                }
                return StatusCode(StatusCodes.Status201Created);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
