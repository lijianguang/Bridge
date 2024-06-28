using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.AMQP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;

namespace Bridge.ActiveMQ
{
    public static class BridgeActiveMQServiceCollectionExtensions
    {
        public static void AddActiveMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ActiveMQProducer, ActiveMQProducer>();

            services.AddTransient<ActiveMQConsumer, ActiveMQConsumer>();

            services.Configure<ActiveMQOptions>(configuration.GetSection(ActiveMQOptions.ActiveMQ));

            services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider() { MaximumRetained = 1000 });

            services.AddSingleton<ObjectPool<Connection>>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ActiveMQOptions>>().Value;
                var policy = new QueueConnectionPooledObjectPolicy(options.QueueUri, options.UserName, options.Password);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            }); 

            services.AddSingleton<ObjectPool<NmsConnection>>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ActiveMQOptions>>().Value;
                var policy = new TopicConnectionPooledObjectPolicy(options.TopicUri, options.UserName, options.Password);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            });

            services.AddSingleton<ObjectPool<Session>>(serviceProvider =>
            {
                var connection = serviceProvider.GetRequiredService<ObjectPool<Connection>>().GetAliveConnection();
                var policy = new QueueSessionPooledObjectPolicy(connection);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            });

            services.AddSingleton<ObjectPool<NmsSession>>(serviceProvider =>
            {
                var connection = serviceProvider.GetRequiredService<ObjectPool<NmsConnection>>().GetAliveConnection();
                var policy = new TopicSessionPooledObjectPolicy(connection);
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                return provider.Create(policy);
            });
        }

        public static void ReleaseActiveMQResource(this IServiceProvider services)
        {
            var sessionPool = services.GetRequiredService<ObjectPool<Session>>();
            var nmsSessionPool = services.GetRequiredService<ObjectPool<NmsSession>>();
            var connectionPool = services.GetRequiredService<ObjectPool<Connection>>();
            var nmsConnectionPool = services.GetRequiredService<ObjectPool<NmsConnection>>();

            if (sessionPool is IDisposable disposableSessionPool)
            {
                disposableSessionPool.Dispose();
            }
            if (nmsSessionPool is IDisposable disposableNmsSessionPool)
            {
                disposableNmsSessionPool.Dispose();
            }
            if (connectionPool is IDisposable disposablePool)
            {
                disposablePool.Dispose();
            }
            if (nmsConnectionPool is IDisposable disposableNmsPool)
            {
                disposableNmsPool.Dispose();
            }
        }
    }
}
