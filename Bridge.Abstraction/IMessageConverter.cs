namespace Bridge
{
    public interface IMessageConverter
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string data);
        object Deserialize(string data, Type type);
    }
}
