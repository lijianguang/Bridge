namespace Bridge.Core
{
    public class MQEndpointFactory : IMQEndpointFactory
    {
        private readonly IHandlerMQDelegateFactory _handlerMQDelegateFactory;

        public MQEndpointFactory(IHandlerMQDelegateFactory handlerMQDelegateFactory)
        {
            _handlerMQDelegateFactory = handlerMQDelegateFactory;
        }

        public MQEndpoint Create(MQHandlerActionDescriptor descriptor)
        {
            return new MQEndpoint(_handlerMQDelegateFactory.CreateRequestDelegate(descriptor));
        }
    }
}
