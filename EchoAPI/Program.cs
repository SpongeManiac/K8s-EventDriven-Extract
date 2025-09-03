using EchoAPI.Controllers;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddRabbitMQClient("rabbitmq");

// Add API Controllers
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

// Setup queue before running
using (var connection = app.Services.GetRequiredService<IConnection>())
using (var channel = await connection!.CreateChannelAsync())
{
    var logger = app.Services.GetRequiredService<ILogger>();
    // Declare the queue asynchronously
    await channel.QueueDeclareAsync(
        queue: "test",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );
    logger.LogInformation("Test queue is ready, running EchoAPI.");
}

app.Run();
