using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    public class QueueSessionPooledObjectPolicy : IPooledObjectPolicy<Session>
    {
        private readonly Connection _connection;

        public QueueSessionPooledObjectPolicy(Connection connection)
        {
            _connection = connection;
        }
        public Session Create()
        {
            if (!_connection.IsStarted)
            {
                _connection.Start();
            }
            return (Session)_connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
        }

        public bool Return(Session obj)
        {
            return true;
        }
    }
}
