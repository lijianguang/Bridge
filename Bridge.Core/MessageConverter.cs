using Newtonsoft.Json;

namespace Bridge.Core
{
    public class MessageConverter : IMessageConverter
    {
        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data) ?? throw new Exception($"Can't deserialize to type {typeof(T).FullName} from data: {data}");
        }

        public object Deserialize(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type) ?? throw new Exception($"Can't deserialize to type {type.FullName} from data: {data}");
        }

        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}
