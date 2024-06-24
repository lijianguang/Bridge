namespace Bridge
{
    public interface IHandlerMQDelegateFactory
    {
        public MQDelegate CreateRequestDelegate(MQHandlerActionDescriptor descriptor);
    }
}
