using Newtonsoft.Json.Linq;
using System.ComponentModel;

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
            if(responseBody.Payload is JToken jtoken)
            {
                return jtoken.ToObject<T>();
            }

            if (responseBody.Payload is null)
            {
                return default;
            }

            if (responseBody.Payload.GetType().Equals(typeof(T)))
            {
                return (T)responseBody.Payload;
            }

            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            if (underlyingType != null)
            {
                return (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(Convert.ChangeType(responseBody.Payload, underlyingType));
            }

            return (T)Convert.ChangeType(responseBody.Payload, typeof(T));

        }
    }
}
