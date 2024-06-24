namespace Bridge
{
    public interface IMQHandlerActionDescriptorProvider
    {
        MQHandlerActionDescriptor Get(MQType mqType, string queueName, string actionName);
        IEnumerable<MQHandlerActionDescriptor> Get(MQType mqType, string queueName);
        IEnumerable<MQHandlerActionDescriptor> Get(MQType mqType);
        IEnumerable<MQHandlerActionDescriptor> Get();
    }
}
