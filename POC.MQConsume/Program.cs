using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using POC.ServiceDefaults.Models.Bogus;
using POC.ServiceDefaults.Models.Config;
using POC.ServiceDefaults.Models.Context;
using POC.ServiceDefaults.Models.Converters;
using POC.ServiceDefaults.Models.Tables;
using RabbitMQ.Client;
using Azure.Core.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

// Load appsettings into config
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<SupabaseConfig>(builder.Configuration.GetSection("supabase"));
SupabaseConfig? config = builder.Configuration.GetSection("supabase").Get<SupabaseConfig>();

var efcore = builder.Services.AddDbContext<SupabaseContext>(options =>
{
    // Make sure to enable NetTopologySuite!
    options.UseNpgsql(config!.SQLConnectionString, o => o.UseNetTopologySuite());
});


// Configure Newtonsoft bc of supabase
builder.Services.Configure<WorkerOptions>(workerOptions =>
{
    var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
    settings.Converters.Add(new ExtractTypeConverter());
    settings.Converters.Add(new ExtractFilterListConverter());
    settings.Converters.Add(new ExtractFilterTypeConverter());
    workerOptions.Serializer = new NewtonsoftJsonObjectSerializer();
});

builder.AddServiceDefaults();

builder.AddRabbitMQClient("rabbitmq");

builder.ConfigureFunctionsWebApplication();

// Add logging early
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

// Add Supabase PostgreSQL Client
builder.Services.AddScoped(sp =>
{
    var connection = new NpgsqlConnection(config!.SQLConnectionString);
    connection.Open();
    return connection;
});
// Add Supabase client as a singleton
builder.Services.AddSingleton((builder) => {
    var client = new Supabase.Client(
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
    var client = serviceProvider.GetRequiredService<Supabase.Client>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Declare the queue asynchronously
        await channel.QueueDeclareAsync(
            queue: "create_extract",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        logger.LogInformation("test channel declared...");
        // Check if mock data exists
        var devices = await client.From<DeviceDTO>().Select("*").Count(Supabase.Postgrest.Constants.CountType.Exact);
        int currentSensorID = 0;
        int currentMeasurementID = 0;
        if (devices == 0)
        {
            // Create Random
            Random random = new Random();
            // Create Fakers
            var deviceFaker = DeviceFaker.GetFaker();
            // Generate 25 Devices
            List<DeviceDTO> mockDevices = DeviceFaker.GetFaker().Generate(25);
            List<SensorDTO> mockSensors = new List<SensorDTO>();
            List<MeasurementDTO> mockMeasurements = new List<MeasurementDTO>();
            foreach (var dev in mockDevices)
            {
                // Create fakers
                var sensorFaker = new SensorFaker(dev.DeviceID, currentSensorID);
                MeasurementFaker measurementFaker;
                // Generate sensors
                var generated_sensors = sensorFaker.Generate(random.Next(1, 4));
                // Keep track of generated sensor ids
                currentSensorID = sensorFaker.currentSensorID;
                mockSensors.AddRange(generated_sensors);

                foreach (var sensor in generated_sensors)
                {
                    measurementFaker = new MeasurementFaker(sensor.SensorID, currentMeasurementID, sensor.SensorType, dev.CreatedAt);
                    mockMeasurements.AddRange(
                        measurementFaker.Generate(1000)
                    );
                    // Keep track of measurement ids
                    currentMeasurementID = measurementFaker.currentMeasurementID;
                }
            }

            // Insert to DB
            logger.LogInformation("Inserting mock devices...");
            await client.From<DeviceDTO>().Insert(mockDevices);
            logger.LogInformation("Inserting mock sensors...");
            await client.From<SensorDTO>().Insert(mockSensors);
            logger.LogInformation("Inserting mock measurements...");
            await client.From<MeasurementDTO>().Insert(mockMeasurements);

            // print details
            logger.LogInformation($"Devices: {mockDevices.Count}");
            logger.LogInformation($"Sensors: {mockSensors.Count}");
            logger.LogInformation($"Measurements: {mockMeasurements.Count}");
        } else
        {
            logger.LogInformation("No need to populate db, skipping...");
        }

        logger.LogInformation("Test queue & DB is ready, running MQConsume.");
    }
    catch(Supabase.Postgrest.Exceptions.PostgrestException ex) {
        logger.LogError(ex, $"Postgresql error whil initializing DB:\n{ex.StatusCode}\n{ex.Source}\n{ex.Message}");
        foreach(var key in ex.Data.Keys)
        {
            logger.LogCritical($"{key}:\t{ex.Data[key]}");
        }
    }
    catch (Exception ex)
    {
        // Unwrap exceptions
        logger.LogError(ex, $"Unknown error while initializing db:\n{ex}\nInner Exception:\n{ex.InnerException}");
        var innerTmp = ex;
        while (innerTmp.InnerException != null) {
            logger.LogError(innerTmp.InnerException, $"Inner Exception of {innerTmp.GetType()}:\n{innerTmp.Message}");
            innerTmp = innerTmp.InnerException;
        }
    }
}

builder.Build().Run();