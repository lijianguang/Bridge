namespace Bridge
{
    public interface IMQMiddlewareFactory
    {
        IMQMiddleware? Create(IServiceProvider serviceProvider, Type mqMiddlewareType);
    }
}
