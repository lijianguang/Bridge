using Bridge;
using Bridge.ActiveMQ;
using Bridge.Core;
using Bridge.Message;
using Bridge.Pub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<PubHostedService>();

        services.AddBridgeServices();

        services.AddActiveMQServices(context.Configuration);

    })
    .Build();

app.Run();