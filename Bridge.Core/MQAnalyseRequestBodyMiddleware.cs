using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bridge.Core
{
    public class MQAnalyseRequestBodyMiddleware : IMQMiddleware
    {
        private readonly IMessageConverter _messageConverter;
        public MQAnalyseRequestBodyMiddleware(IMessageConverter messageConverter)
        {
            _messageConverter = messageConverter;
        }

        public async Task InvokeAsync(MQContext context, MQDelegate next)
        {
            if(context.Message != null)
            {
                context.Request.Body = _messageConverter.Deserialize<RequestBody>(context.Message);
                context.Request.Payload = (type) => {
                    if(context.Request.Body.Payload is JToken jtoken)
                    {
                        return jtoken.ToObject(type);
                    }

                    if(context.Request.Body.Payload is null)
                    {
                        return default;
                    }

                    if (context.Request.Body.Payload.GetType().Equals(type))
                    {
                        return context.Request.Body.Payload;
                    }

                    var underlyingType = Nullable.GetUnderlyingType(type);
                    if(underlyingType != null)
                    {
                        return TypeDescriptor.GetConverter(type).ConvertFrom(Convert.ChangeType(context.Request.Body.Payload, underlyingType));
                    }
                                        
                    return Convert.ChangeType(context.Request.Body.Payload, type);
                };
            }

            await next(context);
        }
    }
}
