using Bridge;
using Bridge.ActiveMQ;
using Bridge.Core;
using Bridge.Pub;
using Bridge.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Sub1;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<PubHostedService>();

        services.AddBridgeServices((publisher) =>
        {
            publisher.OnBefore(request =>
            {
                request.Headers.Add("token","token1");
                request.Headers.Add("marketId", "8");
                Console.WriteLine(JsonConvert.SerializeObject(request));
            });
            publisher.OnAfter(response =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(response));

            });
        });

        services.AddAllHandlerProxies();

        services.AddActiveMQServices(context.Configuration);
        services.AddRabbitMQServices(context.Configuration);

    })
    .Build();

app.Run();