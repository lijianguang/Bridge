using Apache.NMS;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    public static class ObjectPoolExtensions
    {
        public static T GetAlive<T>(this ObjectPool<T> defaultObjectPool) where T : class, IConnection
        {
            var connection = defaultObjectPool.Get();
            if (!connection.IsStarted)
            {
                connection.Start();
            }
            return connection;
        }

        public static void ReturnDead<T>(this ObjectPool<T> defaultObjectPool, T connection) where T : class, IConnection
        {
            if (connection.IsStarted)
            {
                connection.Stop();
            }
            defaultObjectPool.Return(connection);
        }
    }
}
