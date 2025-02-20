using Bridge.ActiveMQ;
using Bridge.Core;
using Bridge.Sub;
using Bridge.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<SubHostedService>();

        services.AddBridgeServices();

        services.AddActiveMQServices(context.Configuration);
        services.AddRabbitMQServices(context.Configuration);

    })
    .Build()
    .RunAsync();