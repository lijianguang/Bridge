using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Core
{
    public class ProducerFactory : IProducerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProducerDescriptorProvider _producerDescriptorProvider;

        public ProducerFactory(IServiceProvider serviceProvider,
            IProducerDescriptorProvider producerDescriptorProvider)
        {
            _serviceProvider = serviceProvider;
            _producerDescriptorProvider = producerDescriptorProvider;
        }

        public IProducer Create(MQType mqType)
        {
            var descriptor = _producerDescriptorProvider.Get(mqType);
            return (IProducer)_serviceProvider.GetRequiredService(descriptor.Type);
        }
    }
}
