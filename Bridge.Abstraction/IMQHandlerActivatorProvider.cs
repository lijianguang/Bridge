namespace Bridge
{
    public interface IMQHandlerActivatorProvider
    {
        Func<IServiceProvider, object> CreateActivator(MQHandlerActionDescriptor descriptor);
    }
}
