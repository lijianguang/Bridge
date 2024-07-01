using Newtonsoft.Json.Linq;

namespace Bridge.Core
{
    public class ReplyMessageProcesser : IReplyMessageProcesser
    {
        public T? Process<T>(ResponseBody responseBody)
        {
            if (responseBody.StatusCode == MQStatusCode.InternalServerError)
            {
                throw responseBody.Payload is JToken jtokenException ? jtokenException.ToObject<Exception>()! : new Exception("Unknown exception.");
            }

            return responseBody.Payload is JToken jtoken ? jtoken.ToObject<T>() : default;
        }
    }
}
