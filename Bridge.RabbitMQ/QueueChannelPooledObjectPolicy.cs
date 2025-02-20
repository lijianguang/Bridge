using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Bridge.RabbitMQ
{
    public class QueueChannelPooledObjectPolicy : IPooledObjectPolicy<IChannel>
    {
        private readonly IConnection _connection;

        public QueueChannelPooledObjectPolicy(IConnection connection)
        {
            _connection = connection;
        }
        public IChannel Create()
        {
            return _connection.CreateChannelAsync().Result;
        }

        public bool Return(IChannel obj)
        {
            return true;
        }
    }
}
