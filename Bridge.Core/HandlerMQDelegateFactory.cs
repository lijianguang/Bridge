namespace Bridge.Core
{
    public class HandlerMQDelegateFactory : IHandlerMQDelegateFactory
    {
        public MQDelegate CreateRequestDelegate(MQHandlerActionDescriptor descriptor)
        {
            return async context =>
            {
                object?[] models;
                var methodParameters = descriptor.ActionMethodInfo.GetParameters();

                if (methodParameters.Any())
                {
                    var methodParameter = methodParameters.First();
                    models = new object?[] { context.Request.Payload(methodParameter.ParameterType) };
                }
                else
                {
                    models = [];
                }

                var invoker = new HandlerActionInvoker(context.RequestServices!, descriptor, models);
                context.Response.NeedReply = context.Request.NeedReply;
                context.Response.Body = await invoker.InvokeAsync();
            };
        }
    }
}
