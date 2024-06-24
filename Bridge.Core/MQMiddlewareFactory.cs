using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Core
{
    public class MQMiddlewareFactory : IMQMiddlewareFactory
    {
        public IMQMiddleware? Create(IServiceProvider serviceProvider, Type mqMiddlewareType)
        {
            return (IMQMiddleware)ActivatorUtilities.CreateInstance(serviceProvider, mqMiddlewareType);
        }
    }
}
