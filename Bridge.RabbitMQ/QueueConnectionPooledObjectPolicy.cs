using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Bridge.RabbitMQ
{
    public class QueueConnectionPooledObjectPolicy : IPooledObjectPolicy<IConnection>
    {
        private readonly string _hostName;
        private readonly int? _port;
        private readonly string _userName;
        private readonly string _password;

        public QueueConnectionPooledObjectPolicy(string hostName, int? port,string userName, string password)
        {
            _hostName = hostName;
            _port = port;
            _userName = userName;
            _password = password;
        }
        public IConnection Create()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName
            };
            if(_port != null && _port.HasValue && _port != 0)
            {
                factory.Port = _port.Value;
            }
            if (!string.IsNullOrEmpty(_userName))
            {
                factory.UserName = _userName;
            }
            if (!string.IsNullOrEmpty(_password))
            {
                factory.Password = _password;
            }

            return factory.CreateConnectionAsync().Result;
        }

        public bool Return(IConnection obj)
        {
            return true;
        }
    }
}
