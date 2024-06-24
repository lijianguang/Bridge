namespace Bridge
{
    public interface IMQEndpointFactory
    {
        MQEndpoint Create(MQHandlerActionDescriptor descriptor);
    }
}
