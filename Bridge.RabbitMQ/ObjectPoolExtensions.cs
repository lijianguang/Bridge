using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Bridge.RabbitMQ
{
    public static class ObjectPoolExtensions
    {
        public static T GetAliveConnection<T>(this ObjectPool<T> defaultObjectPool) where T : class, IConnection
        {
            return defaultObjectPool.Get();
        }
        public static void ReturnSuspendedConnection<T>(this ObjectPool<T> defaultObjectPool, T connection) where T : class, IConnection
        {
            defaultObjectPool.Return(connection);
        }

        public static T GetAliveChannel<T>(this ObjectPool<T> defaultObjectPool) where T : class, IChannel
        {
            var channel = defaultObjectPool.Get();
           
            return channel;
        }
        public static void ReturnSuspendedChannel<T>(this ObjectPool<T> defaultObjectPool, T session) where T : class, IChannel
        {
            defaultObjectPool.Return(session);
        }
    }
}
