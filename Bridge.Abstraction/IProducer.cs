namespace Bridge
{
    public interface IProducer
    {
        Task SendAsync(string queueName, string message);
        Task SendMulticastAsync(string queueName, string message);
        Task<string> SendAndWaitReplyAsync(string queueName, string message);
    }
}
