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
                    string correlationId = Guid.NewGuid().ToString();
                    requestMessage.NMSCorrelationID = correlationId;
                    using (IMessageProducer producer = await session.CreateProducerAsync())
                    {
                        NmsDestinationAccessor destinationResolver = new NmsDestinationAccessor();
                        IDestination destination = destinationResolver.ResolveDestinationName(session, queueName);
                        await producer.SendAsync(destination, requestMessage);
                    }
                    DateTime utcNow = DateTime.UtcNow;
                    IMessage reply = await consumer.ReceiveAsync(TimeSpan.FromSeconds(20));
                    if (reply == null)
                    {
                        throw new Exception($"Request Timeout. Waiting from {utcNow} to {DateTime.UtcNow}, Queue: {queueName}, NMSCorrelationID: {requestMessage.NMSCorrelationID}");
                    }
                    ITextMessage replyMessage = (ITextMessage)reply;
                    Console.WriteLine(replyMessage.Text);
                    return replyMessage.Text;
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
                    Console.WriteLine("Message sent: " + textMessage.Text);
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
                    Console.WriteLine("Message sent: " + textMessage.Text);
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
