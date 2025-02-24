using Bridge.ActiveMQ;
using Bridge.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server1;

namespace Bridge.Client
{
    public class ClientHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public ClientHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var queue1HandlerProxy = _serviceProvider.GetRequiredService<ActiveMQ_Queue1_Proxy>();
            var queue3HandlerProxy = _serviceProvider.GetRequiredService<Server1.ActiveMQ_Queue3Multicast_Proxy>();
            var queue5HandlerProxy = _serviceProvider.GetRequiredService<RabbitMQ_Queue5_Proxy>();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            _serviceProvider.ReleaseRabbitMQResource();
            return Task.CompletedTask;
        }
    }
}
