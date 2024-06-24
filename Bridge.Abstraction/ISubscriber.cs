namespace Bridge
{
    public interface ISubscriber : IDisposable
    {
        Task SubscribeAsync(MQType mqType, string queueName, Func<string, Task<(bool, object?)>> processMQMessageAsync);
        Task SubscribeMulticastAsync(MQType mqType, string queueName, Func<string, Task<(bool, object?)>> processMQMessageAsync);
    }
}
