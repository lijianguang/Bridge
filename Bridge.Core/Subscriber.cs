namespace Bridge.Core
{
    public class Subscriber : ISubscriber
    {
        private readonly IConsumerFactory _consumerFactory;
        private readonly IMessageConverter _messageConverter;
        private readonly Dictionary<(MQType, string), IConsumer> _consumers = new Dictionary<(MQType, string), IConsumer>();

        public Subscriber(IConsumerFactory consumerFactory, IMessageConverter messageConverter)
        {
            _consumerFactory = consumerFactory;
            _messageConverter = messageConverter;
        }

        public void Dispose()
        {
            foreach (var consumer in _consumers)
            {
                consumer.Value.Dispose();
            }
        }

        private IConsumer ResolveConsumer(MQType mqType, string queueName)
        {
            lock (this)
            {
                if (!_consumers.TryGetValue((mqType, queueName), out var consumer))
                {
                    consumer = _consumerFactory.Create(mqType);
                    _consumers.Add((mqType, queueName), consumer);
                }
                return consumer;
            }
        }

        public async Task SubscribeAsync(MQType mqType, string queueName, Func<string, Task<(bool, ResponseBody?)>> processMQMessageAsync)
        {
            var consumer = ResolveConsumer(mqType, queueName);
            await consumer.ReceiveAsync(queueName, async (message) => {
                var result = await processMQMessageAsync(message);
                return (result.Item1, _messageConverter.Serialize(result.Item2));
            });
        }

        public async Task SubscribeMulticastAsync(MQType mqType, string queueName, Func<string, Task<(bool, ResponseBody?)>> processMQMessageAsync)
        {
            var consumer = ResolveConsumer(mqType, queueName);
            await consumer.ReceiveMulticastAsync(queueName, async (message) => {
                var result = await processMQMessageAsync(message);
                return (result.Item1, _messageConverter.Serialize(result.Item2));
            });
        }
    }
}
