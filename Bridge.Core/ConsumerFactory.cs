using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Core
{
    public class ConsumerFactory : IConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsumerDescriptorProvider _consumerDescriptorProvider;

        public ConsumerFactory(IServiceProvider serviceProvider, 
            IConsumerDescriptorProvider consumerDescriptorProvider)
        {
            _serviceProvider = serviceProvider;
            _consumerDescriptorProvider = consumerDescriptorProvider;
        }

        public IConsumer Create(MQType mqType)
        {
            var descriptor = _consumerDescriptorProvider.Get(mqType);
            return (IConsumer)_serviceProvider.GetRequiredService(descriptor.Type);
        }
    }
}
