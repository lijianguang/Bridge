using Newtonsoft.Json.Linq;

namespace Bridge.Core
{
    public class ReplyMessageProcesser : IReplyMessageProcesser
    {
        private readonly IMessageConverter _messageConverter;

        public ReplyMessageProcesser(IMessageConverter messageConverter)
        {
            _messageConverter = messageConverter;
        }

        public T? Process<T>(string replyMessage)
        {
            var responseBody = _messageConverter.Deserialize<ResponseBody>(replyMessage);

            if (responseBody.StatusCode == MQStatusCode.InternalServerError)
            {
                throw responseBody.Payload is JToken jtokenException ? jtokenException.ToObject<Exception>()! : new Exception("Unknown exception.");
            }

            return responseBody.Payload is JToken jtoken ? jtoken.ToObject<T>() : default;
        }
    }
}
