using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Core
{
    public class Launcher : ILauncher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISubscriber _subscriber;
        private readonly IMQHandlerActionDescriptorProvider _mqActionDescriptorProvider;

        public Launcher(IServiceProvider serviceProvider, 
            ISubscriber subscriber, 
            IMQHandlerActionDescriptorProvider mqActionDescriptorProvider)
        {
            _serviceProvider = serviceProvider;
            _subscriber = subscriber;
            _mqActionDescriptorProvider = mqActionDescriptorProvider;
        }

        private async Task<(bool, object?)> ProcessMQMessageAsync(MQType mqType, string queueName, string message, MQDelegate pipelineEntry)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var mqContextBuilder = new MQContextBuilder(mqType, queueName);
                mqContextBuilder.Configure((context) =>
                {
                    context.RequestServices = scope.ServiceProvider;
                    context.Request.Body = message;
                });
                var context = mqContextBuilder.Build();
                await Task.Factory.StartNew(async () => await pipelineEntry(context), default, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
                return (context.Response.NeedReply, context.Response.Body);
            }
        }

        public Task LaunchAsync(MQType mqType, string queueName, bool isMulticast, MQDelegate pipelineEntry)
        {
            return SubscribeAsync(mqType, queueName, isMulticast, pipelineEntry);
        }

        public Task LaunchAsync(MQType mqType, MQDelegate pipelineEntry)
        {
            var queues = _mqActionDescriptorProvider.Get(mqType).Select(x => (x.QueueName, x.IsMulticast)).Distinct().ToList();
            foreach (var queue in queues)
            {
                LaunchAsync(mqType, queue.QueueName, queue.IsMulticast, pipelineEntry);
            }
            return Task.CompletedTask;
        }

        public Task LaunchAsync(MQDelegate pipelineEntry)
        {
            var mqTypes = _mqActionDescriptorProvider.Get().Select(x => x.MQType).Distinct().ToList();
            foreach (var mqType in mqTypes)
            {
                LaunchAsync(mqType, pipelineEntry);
            }
            return Task.CompletedTask;
        }

        private Task SubscribeAsync(MQType mqType, string queueName, bool isMulticast, MQDelegate pipelineEntry)
        {
            if (isMulticast)
            {
                return Task.Factory.StartNew(async () => await _subscriber.SubscribeMulticastAsync(mqType, queueName, async (message) => await ProcessMQMessageAsync(mqType, queueName, message, pipelineEntry)), default, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
            return Task.Factory.StartNew(async () => await _subscriber.SubscribeAsync(mqType, queueName, async (message) => await ProcessMQMessageAsync(mqType, queueName, message, pipelineEntry)), default, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public Task StopAsync()
        {
            _subscriber.Dispose();
            return Task.CompletedTask;
        }
    }
}
