using POC.ServiceDefaults.Models.Converters;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.AddRabbitMQClient("rabbitmq");

// Add API Controllers & Custom Converters
//builder.Services.AddControllers().AddJsonOptions(opts =>
//{
//    opts.JsonSerializerOptions.Converters.Add(new ExtractFilterConverter());
//});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
    

// Setup queue before running
//using (var connection = app.Services.GetRequiredService<IConnection>())
//using (var channel = await connection!.CreateChannelAsync())
//{
//    var logger = app.Services.GetRequiredService<ILogger<Program>>();
//    // Declare the queue asynchronously
//    await channel.QueueDeclareAsync(
//        queue: "create_extract",
//        durable: true,
//        exclusive: false,
//        autoDelete: false,
//        arguments: null
//    );
//    logger.LogInformation("Test queue is ready, running EchoAPI.");
//}

app.Run();
