using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using POC.ServiceDefaults.Models.Bogus;
using POC.ServiceDefaults.Models.Config;
using POC.ServiceDefaults.Models.Interfaces;
using POC.ServiceDefaults.Models.Tables;
using RabbitMQ.Client;
using ServiceDefaults.Models.Bogus;
using Supabase;
using System.Net.Security;

var builder = FunctionsApplication.CreateBuilder(args);

// Load appsettings into config
builder.Configuration.AddJsonFile("appsettings.json");
// Bind options from json to SupabaseConfig object
builder.Services.Configure<SupabaseConfig>(builder.Configuration.GetSection("supabase"));

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq");

builder.ConfigureFunctionsWebApplication();

// Add logging early
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Add Supabase client as a singleton
builder.Services.Configure<SupabaseConfig>(builder.Configuration.GetSection("supabase"));
SupabaseConfig? config = builder.Configuration.GetSection("supabase").Get<SupabaseConfig>();
builder.Services.AddSingleton((builder) => {
    var client = new Client(
        config?.Host ?? "unknown",
        config?.Key ?? "unknown"
    );
    var task = client.InitializeAsync();
    task.Wait();
    return client;
});

// Build a temporary service provider to run the setup
using (var serviceProvider = builder.Services.BuildServiceProvider())
using (var connection = serviceProvider.GetRequiredService<IConnection>())
using (var channel = await connection!.CreateChannelAsync())
{
    var client = serviceProvider.GetRequiredService<Client>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    // Declare the queue asynchronously
    await channel.QueueDeclareAsync(
        queue: "test",
        durable: true,
        exclusive: false,
        autoDelete: false,
        arguments: null
    );
    // Check if mock data exists
    var devices = await client.From<Device>().Select("*").Count(Supabase.Postgrest.Constants.CountType.Exact);
    if (devices == 0)
    {
        // Create Random
        Random random = new Random();
        // Create Fakers
        var deviceFaker = DeviceFaker.GetFaker();
        // Generate 25 Devices
        List<Device> mockDevices = DeviceFaker.GetFaker().Generate(25);
        List<ISensor> mockSensors = new List<ISensor>();
        List<ISensorMeasurement> mockMeasurements = new List<ISensorMeasurement>();
        foreach (var dev in  mockDevices)
        {
            // Create fakers
            var sensorFaker = SensorFaker.GetFaker(dev.DeviceID);
            var measurementFaker = new MeasurementFaker(0, SensorType.Unknown, DateTime.UnixEpoch);
            
            var generated_sensors = sensorFaker.Generate(random.Next(1,4));
            foreach(var sensor in generated_sensors)
            {
                measurementFaker.sensorID = sensor.SensorID;
                measurementFaker.sensorType = sensor.SensorType;
                measurementFaker.deviceCreation = dev.CreatedAt;
                mockMeasurements.AddRange(
                    measurementFaker.Generate(1000)
                );
            }

            // print details
            logger.LogInformation($"Devices: {mockDevices.Count}");
            logger.LogInformation($"Sensors: {mockSensors.Count}");
            logger.LogInformation($"Measurements: {mockMeasurements.Count}");
        }
    }

    logger.LogInformation("Test queue & DB is ready, running MQConsume.");
}

builder.Build().Run();