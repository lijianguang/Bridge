namespace Bridge.ActiveMQ
{
    public class ActiveMQOptions
    {
        public const string ActiveMQ = "ActiveMQ";

        public string QueueUri { get; set; } = string.Empty;
        public string TopicUri { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
