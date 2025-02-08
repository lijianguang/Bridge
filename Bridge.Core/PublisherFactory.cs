using Bridge.Abstraction;
namespace Bridge.Core
{
    public class PublisherFactory : IPublisherFactory
    {
        private readonly IProducerFactory _mqProducerFactory;
        private readonly IMessageConverter _messageConverter;
        private readonly IReplyMessageProcesser _replyMessageProcesser;

        public PublisherFactory(IProducerFactory mqProducerFactory, IMessageConverter messageConverter, IReplyMessageProcesser replyMessageProcesser)
        {
            _mqProducerFactory = mqProducerFactory;
            _messageConverter = messageConverter;
            _replyMessageProcesser = replyMessageProcesser;
        }

        public IPublisher GetPublisher()
        {
            return new Publisher(_mqProducerFactory, _messageConverter, _replyMessageProcesser);
        }
    }
}
