using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POC.ServiceDefaults.Models.Context;

var builder = DistributedApplication.CreateBuilder(args);

var mq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

var consumer = builder.AddAzureFunctionsProject<Projects.POC_MQConsume>("mqconsume")
    .WaitFor(mq)
    .WithReference(mq);

builder.AddProject<Projects.POC_ExtractTriggerAPI>("trigger")
    .WaitFor(consumer)
    .WithReference(mq);

builder.Build().Run();
