namespace Bridge.Core
{
    public class MQEndpointMiddleware : IMQMiddleware
    {
        public async Task InvokeAsync(MQContext context, MQDelegate next)
        {
            var endpoint = context.Endpoint;

            if (endpoint != null)
            {
                var mqDelegate = endpoint.MQDelegate;
                if (mqDelegate != null)
                {
                    await mqDelegate(context);
                }
            }
            await next(context);
        }
    }
}
