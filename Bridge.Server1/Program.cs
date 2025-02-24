using Bridge.ActiveMQ;
using Bridge.Core;
using Bridge.Server1;
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
        services.AddHostedService<Server1HostedService>();

        services.AddBridgeServices();

        services.AddActiveMQServices(context.Configuration);
        services.AddRabbitMQServices(context.Configuration);

    })
    .Build()
    .RunAsync();