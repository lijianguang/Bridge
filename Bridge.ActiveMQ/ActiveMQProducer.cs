﻿using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;
using Spring.Messaging.Nms.Support.Destinations;

namespace Bridge.ActiveMQ
{
    [ProducerMapping(MQType.ActiveMQ)]
    public class ActiveMQProducer : IProducer
    {
        private readonly ObjectPool<Connection> _connectionPool;
        private readonly ObjectPool<NmsConnection> _nmsconnectionPool;
        public ActiveMQProducer(ObjectPool<Connection> connectionPool, 
            ObjectPool<NmsConnection> nmsconnectionPool)
        {
            _connectionPool = connectionPool;
            _nmsconnectionPool = nmsconnectionPool;
        }

        public async Task<string> SendAndWaitReplyAsync(string queueName, string message)
        {
            var connection = _connectionPool.GetAlive();
            try
            {
                using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                throw;
            }
            finally
            {
                _connectionPool.ReturnDead(connection);
            }
        }

        public async Task SendAsync(string queueName, string message)
        {
            var connection = _connectionPool.GetAlive();
            try
            {
                using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = await session.GetQueueAsync(queueName);
                    using (IMessageProducer producer = await session.CreateProducerAsync(destination))
                    {
                        ITextMessage textMessage = await producer.CreateTextMessageAsync(message);
                        await producer.SendAsync(textMessage);
                        Console.WriteLine("Message sent: " + textMessage.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _connectionPool.ReturnDead(connection);
            }
            
        }

        public async Task SendMulticastAsync(string queueName, string message)
        {
            var connection = _nmsconnectionPool.GetAlive();
            try
            {
                using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = await session.GetTopicAsync($"topic://{queueName}");
                    using (IMessageProducer producer = await session.CreateProducerAsync(destination))
                    {
                        ITextMessage textMessage = await producer.CreateTextMessageAsync(message);
                        await producer.SendAsync(textMessage);
                        Console.WriteLine("Message sent: " + textMessage.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _nmsconnectionPool.Return(connection);
            }
        }
    }
}
