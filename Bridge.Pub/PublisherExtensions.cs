using Newtonsoft.Json;

namespace Bridge.Pub
{
    public static class PublisherExtensions
    {
        public static void LogToConsole(this IPublisher publisher)
        {
            publisher.OnBefore(req =>
            {
                Console.WriteLine($"Request: {JsonConvert.SerializeObject(req)}");
            });
            publisher.OnAfter(res =>
            {
                Console.WriteLine($"Response: {JsonConvert.SerializeObject(res)}");
            });
        }
        public static async Task ActiveMQ_PublishAsync<T>(this IPublisher publisher, string queueName, string actionName, T? data)
        {
            publisher.LogToConsole();
            await publisher.PublishAsync(MQType.ActiveMQ, queueName, actionName, data);
        }
        public static async Task ActiveMQ_PublishAsync(this IPublisher publisher, string queueName, string actionName)
        {
            publisher.LogToConsole();
            await publisher.PublishAsync(MQType.ActiveMQ, queueName, actionName);
        }

        public static async Task<TR?> ActiveMQ_PublishAndWaitReplyAsync<T, TR>(this IPublisher publisher, string queueName, string actionName, T? data)
        {
            publisher.LogToConsole();
            return await publisher.PublishAndWaitReplyAsync<T, TR>(MQType.ActiveMQ, queueName, actionName, data);
        }
        public static async Task<TR?> ActiveMQ_PublishAndWaitReplyAsync<TR>(this IPublisher publisher, string queueName, string actionName)
        {
            publisher.LogToConsole();
            return await publisher.PublishAndWaitReplyAsync<TR>(MQType.ActiveMQ, queueName, actionName);
        }

        public static async Task ActiveMQ_PublishMulticastAsync<T>(this IPublisher publisher, string queueName, string actionName, T? data)
        {
            publisher.LogToConsole();
            await publisher.PublishMulticastAsync(MQType.ActiveMQ, queueName, actionName, data);
        }
        public static async Task ActiveMQ_PublishMulticastAsync(this IPublisher publisher, string queueName, string actionName)
        {
            publisher.LogToConsole();
            await publisher.PublishMulticastAsync(MQType.ActiveMQ, queueName, actionName);
        }
    }
}
