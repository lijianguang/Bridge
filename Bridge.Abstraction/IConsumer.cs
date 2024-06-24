namespace Bridge
{
    public interface IConsumer : IDisposable
    {
        Task ReceiveAsync(string queueName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback);

        Task ReceiveMulticastAsync(string topicName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback);
    }
}
