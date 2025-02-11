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

                var invoker = new HandlerActionInvoker(context, descriptor, models);
                context.Response.Body = new ResponseBody() { Payload =  await invoker.InvokeAsync(), StatusCode = MQStatusCode.OK };
            };
        }
    }
}
