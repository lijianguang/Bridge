using Apache.NMS;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    public class TopicConnectionPooledObjectPolicy : IPooledObjectPolicy<NmsConnection>
    {
        private readonly string _uri;
        private readonly string _userName;
        private readonly string _password;

        public TopicConnectionPooledObjectPolicy(string uri, string userName, string password)
        {
            _uri = uri;
            _userName = userName;
            _password = password;
        }
        public NmsConnection Create()
        {
            var factory = new NMSConnectionFactory(_uri);
            return (NmsConnection)factory.CreateConnection(_userName, _password);
        }

        public bool Return(NmsConnection obj)
        {
            return true;
        }
    }
}
