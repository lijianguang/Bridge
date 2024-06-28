using Amqp;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    public static class ObjectPoolExtensions
    {
        public static T GetAliveConnection<T>(this ObjectPool<T> defaultObjectPool) where T : class, Apache.NMS.IConnection
        {
            var connection = defaultObjectPool.Get();
            if (!connection.IsStarted)
            {
                connection.Start();
            }
            return connection;
        }
        public static void ReturnSuspendedConnection<T>(this ObjectPool<T> defaultObjectPool, T connection) where T : class, Apache.NMS.IConnection
        {
            if (connection.IsStarted)
            {
                connection.Stop();
            }
            defaultObjectPool.Return(connection);
        }

        public static T GetAliveSession<T>(this ObjectPool<T> defaultObjectPool) where T : Apache.NMS.ActiveMQ.Session
        {
            var session = defaultObjectPool.Get();
            if (!session.Started)
            {
                session.Start();
            }
            return session;
        }
        public static void ReturnSuspendedSession<T>(this ObjectPool<T> defaultObjectPool, T session) where T : Apache.NMS.ActiveMQ.Session
        {
            if (session.Started)
            {
                session.Stop();
            }
            defaultObjectPool.Return(session);
        }
        
        public static T GetAliveNmsSession<T>(this ObjectPool<T> defaultObjectPool) where T : NmsSession
        {
            var session = defaultObjectPool.Get();
            if (!session.IsStarted)
            {
                session.Start();
            }
            return session;
        }
        public static void ReturnSuspendedNmsSession<T>(this ObjectPool<T> defaultObjectPool, T session) where T : NmsSession
        {
            if (session.IsStarted)
            {
                session.Stop();
            }
            defaultObjectPool.Return(session);
        }
    }
}
