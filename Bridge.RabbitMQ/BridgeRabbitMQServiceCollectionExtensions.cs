using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Bridge.RabbitMQ
{
    public static class BridgeRabbitMQServiceCollectionExtensions
    {
        public static void AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<RabbitMQProducer, RabbitMQProducer>();

            services.AddTransient<RabbitMQConsumer, RabbitMQConsumer>();

            services.Configure<RabbitMQOptions>(configuration.GetSection(RabbitMQOptions.RabbitMQ));

            services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider() { MaximumRetained = 1000 });

            services.AddSingleton<ObjectPool<IConnection>>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                var policy = new QueueConnectionPooledObjectPolicy(options.HostName, options.Port, options.UserName, options.Password);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            }); 

            services.AddSingleton<ObjectPool<IChannel>>(serviceProvider =>
            {
                var connection = serviceProvider.GetRequiredService<ObjectPool<IConnection>>().GetAliveConnection();
                var policy = new QueueChannelPooledObjectPolicy(connection);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            });
        }

        public static void ReleaseRabbitMQResource(this IServiceProvider services)
        {
            var channelPool = services.GetRequiredService<ObjectPool<IChannel>>();
            var connectionPool = services.GetRequiredService<ObjectPool<IConnection>>();

            if (channelPool is IDisposable disposableChannelPool)
            {
                disposableChannelPool.Dispose();
            }
            if (connectionPool is IDisposable disposablePool)
            {
                disposablePool.Dispose();
            }
        }
    }
}
