using Apache.NMS;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    public class TopicSessionPooledObjectPolicy : IPooledObjectPolicy<NmsSession>
    {
        private readonly NmsConnection _connection;

        public TopicSessionPooledObjectPolicy(NmsConnection connection)
        {
            _connection = connection;
        }
        public NmsSession Create()
        {
            if (!_connection.IsStarted)
            {
                _connection.Start();
            }
            return (NmsSession)_connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        }

        public bool Return(NmsSession obj)
        {
            return true;
        }
    }
}
