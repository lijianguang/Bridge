using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Core
{
    public class MQHandlerActivatorProvider : IMQHandlerActivatorProvider
    {
        public Func<IServiceProvider, object> CreateActivator(MQHandlerActionDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            var handlerType = descriptor.HandlerType;
            if (handlerType == null)
            {
                throw new ArgumentException($"Can't find the handler.");
            }

            var typeActivator = ActivatorUtilities.CreateFactory(handlerType, Type.EmptyTypes);
            return serviceProvider => typeActivator(serviceProvider, arguments: null);
        }
    }
}
