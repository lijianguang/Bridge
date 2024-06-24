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
            if(context.Request.Body != null)
            {
                var body = _messageConverter.Deserialize<MessageBody>(context.Request.Body);
                context.Request.NeedReply = body.NeedReply;
                context.Request.Payload = (type) =>  body.Payload is JToken jtoken ? jtoken.ToObject(type) : default;
                context.Request.ActionName = body.ActionName;
            }

            return next(context);
        }
    }
}
