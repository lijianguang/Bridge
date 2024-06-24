namespace Bridge.Core
{
    public class MQEndpointMiddleware : IMQMiddleware
    {
        public Task InvokeAsync(MQContext context, MQDelegate next)
        {
            var endpoint = context.Endpoint;

            if (endpoint != null)
            {
                var mqDelegate = endpoint.MQDelegate;
                if (mqDelegate != null)
                {
                    return mqDelegate(context);
                }
            }
            return next(context);
        }
    }
}
