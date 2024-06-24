using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Bridge.ActiveMQ
{
    public class QueueConnectionPooledObjectPolicy : IPooledObjectPolicy<Connection>
    {
        private readonly string _uri;
        private readonly string _userName;
        private readonly string _password;

        public QueueConnectionPooledObjectPolicy(string uri, string userName, string password)
        {
            _uri = uri;
            _userName = userName;
            _password = password;
        }
        public Connection Create()
        {
            IConnectionFactory factory = new ConnectionFactory(_uri);
            return (Connection)factory.CreateConnection(_userName, _password);
        }

        public bool Return(Connection obj)
        {
            return true;
        }
    }
}
