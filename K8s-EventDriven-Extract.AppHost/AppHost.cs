var builder = DistributedApplication.CreateBuilder(args);

var mq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

builder.AddAzureFunctionsProject<Projects.MQConsume>("mqconsume")
    .WaitFor(mq)
    .WithReference(mq);

builder.AddProject<Projects.ExtractTriggerAPI>("trigger")
    .WaitFor(mq)
    .WithReference(mq);

builder.Build().Run();
