namespace Bridge.Core
{
    public class MQHandlerFactoryProvider : IMQHandlerFactoryProvider
    {
        private readonly IMQHandlerActivatorProvider _mqHandlerActivatorProvider;

        public MQHandlerFactoryProvider(IMQHandlerActivatorProvider mqHandlerActivatorProvider) 
        {
            _mqHandlerActivatorProvider = mqHandlerActivatorProvider;
        }

        public Func<IServiceProvider, object> CreateControllerFactory(MQHandlerActionDescriptor descriptor)
        {
            ArgumentNullException.ThrowIfNull(descriptor);

            var handlerType = descriptor.HandlerType;
            if (handlerType == null)
            {
                throw new ArgumentException($"Can't find the handler.");
            }


            var handlerActivator = _mqHandlerActivatorProvider.CreateActivator(descriptor);
            return (IServiceProvider serviceProvider) =>
            {
                return handlerActivator(serviceProvider);
            };
        }
    }
}
