namespace Bridge.Core
{
    public class MQEndpointRoutingMiddleware : IMQMiddleware
    {
        private readonly IMQEndpointFactory _endpointFactory;
        private readonly IMQHandlerActionDescriptorProvider _mqHandlerActionDescriptorProvider;

        public MQEndpointRoutingMiddleware(IMQEndpointFactory endpointFactory,
            IMQHandlerActionDescriptorProvider mqHandlerActionDescriptorProvider) 
        {
            _endpointFactory = endpointFactory;
            _mqHandlerActionDescriptorProvider = mqHandlerActionDescriptorProvider;
        }

        public Task InvokeAsync(MQContext context, MQDelegate next)
        {
            var endpoint = context.Endpoint;

            if (endpoint != null)
            {
                return next(context);
            }
            if(context.Request.ActionName == null)
            {
                throw new Exception("Can't find ActionName.");
            }
            context.Endpoint = _endpointFactory.Create(_mqHandlerActionDescriptorProvider.Get(context.Request.MQType, context.Request.QueueName, context.Request.ActionName));
            return next(context);
        }
    }
}
