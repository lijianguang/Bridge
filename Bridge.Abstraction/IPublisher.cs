namespace Bridge
{
    public interface IPublisher
    {
        Task PublishAsync<T>(MQType mqType, string queueName, string actionName, T? data);
        Task PublishAsync(MQType mqType, string queueName, string actionName);

        Task PublishMulticastAsync<T>(MQType mqType, string queueName, string actionName, T? data);
        Task PublishMulticastAsync(MQType mqType, string queueName, string actionName);

        Task<TR?> PublishAndWaitReplyAsync<T, TR>(MQType mqType, string queueName, string actionName, T? data);
        Task<TR?> PublishAndWaitReplyAsync<TR>(MQType mqType, string queueName, string actionName);
    }
}
