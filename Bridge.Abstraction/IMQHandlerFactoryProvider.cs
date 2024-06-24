namespace Bridge
{
    public interface IMQHandlerFactoryProvider
    {
        Func<IServiceProvider, object> CreateControllerFactory(MQHandlerActionDescriptor descriptor);
    }
}
