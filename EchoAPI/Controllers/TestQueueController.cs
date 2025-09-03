using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EchoAPI.Controllers
{
    [ApiController]
    [Route("api/addmessage")]
    public class TestQueueController : ControllerBase
    {
        private readonly ILogger<TestQueueController> _logger;
        private readonly IConnection _mqConnection;

        public TestQueueController(
            ILogger<TestQueueController> logger,
            IConnection connection
        )
        {
            _logger = logger;
            _mqConnection = connection;
        }

        [HttpPost(Name = "AddMessage")]
        [Consumes("application/json")]
        public async Task<IActionResult> Post(
            [FromBody] string message
        )
        {
            try
            {
                using (var channel = await _mqConnection.CreateChannelAsync())
                {
                    // Send message to queue
                    await channel.BasicPublishAsync(string.Empty, "test", body: Encoding.UTF8.GetBytes(message));
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
