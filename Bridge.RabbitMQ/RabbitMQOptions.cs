namespace Bridge.RabbitMQ
{
    public class RabbitMQOptions
    {
        public const string RabbitMQ = "RabbitMQ";

        public string HostName { get; set; } = string.Empty;
        public int? Port { get; set; } = null;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
