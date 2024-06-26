using Newtonsoft.Json.Linq;

namespace Bridge.Core
{
    public class MQAnalyseRequestBodyMiddleware : IMQMiddleware
    {
        private readonly IMessageConverter _messageConverter;
        public MQAnalyseRequestBodyMiddleware(IMessageConverter messageConverter)
        {
            _messageConverter = messageConverter;
        }

        public Task InvokeAsync(MQContext context, MQDelegate next)
        {
            if(context.Message != null)
            {
                context.Request.Body = _messageConverter.Deserialize<RequestBody>(context.Message);
                context.Request.Payload = (type) => context.Request.Body.Payload is JToken jtoken ? jtoken.ToObject(type) : default;
            }

            return next(context);
        }
    }
}
