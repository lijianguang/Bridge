using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;
using Spring.Messaging.Nms.Support.Destinations;

namespace Bridge.ActiveMQ
{
    [ProducerMapping(MQType.ActiveMQ)]
    public class ActiveMQProducer : IProducer
    {
        private readonly ObjectPool<Session> _sessionPool;
        private readonly ObjectPool<NmsSession> _nmsSessionPool;
        public ActiveMQProducer(ObjectPool<Session> sessionPool,
            ObjectPool<NmsSession> nmsSessionPool)
        {
            _sessionPool = sessionPool;
            _nmsSessionPool = nmsSessionPool;
        }

        public async Task<string> SendAndWaitReplyAsync(string queueName, string message)
        {
            var session = _sessionPool.GetAliveSession();
            try
            {
                ITemporaryQueue queue = session.CreateTemporaryQueue();
                using (IMessageConsumer consumer = await session.CreateConsumerAsync(queue))
                {
                    ITextMessage requestMessage = await session.CreateTextMessageAsync(message);
                    requestMessage.NMSReplyTo = queue;
                    requestMessage.NMSCorrelationID = Guid.NewGuid().ToString();
                    using (IMessageProducer producer = await session.CreateProducerAsync())
                    {
                        await producer.SendAsync(new NmsDestinationAccessor().ResolveDestinationName(session, queueName), 
                            requestMessage);
                    }
                    DateTime utcNow = DateTime.UtcNow;
                    IMessage reply = await consumer.ReceiveAsync(TimeSpan.FromSeconds(20));
                    if (reply == null)
                    {
                        throw new Exception($"Request Timeout. Waiting from {utcNow} to {DateTime.UtcNow}, Queue: {queueName}, NMSCorrelationID: {requestMessage.NMSCorrelationID}");
                    }
                    return ((ITextMessage)reply).Text;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                throw;
            }
            finally
            {
                _sessionPool.ReturnSuspendedSession(session);
            }
        }

        public async Task SendAsync(string queueName, string message)
        {
            var session = _sessionPool.GetAliveSession();
            try
            {
                IDestination destination = await session.GetQueueAsync(queueName);
                using (IMessageProducer producer = await session.CreateProducerAsync(destination))
                {
                    ITextMessage textMessage = await producer.CreateTextMessageAsync(message);
                    await producer.SendAsync(textMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _sessionPool.ReturnSuspendedSession(session);
            }
            
        }

        public async Task SendMulticastAsync(string queueName, string message)
        {
            var nmsSession = _nmsSessionPool.GetAliveNmsSession();
            try
            {
                IDestination destination = await nmsSession.GetTopicAsync($"topic://{queueName}");
                using (IMessageProducer producer = await nmsSession.CreateProducerAsync(destination))
                {
                    ITextMessage textMessage = await producer.CreateTextMessageAsync(message);
                    await producer.SendAsync(textMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _nmsSessionPool.ReturnSuspendedNmsSession(nmsSession);
            }
        }
    }
}
