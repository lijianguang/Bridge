using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.AMQP;
using Microsoft.Extensions.ObjectPool;

namespace Bridge.ActiveMQ
{
    [ConsumerMapping(MQType.ActiveMQ)]
    public class ActiveMQConsumer : IConsumer
    {
        private readonly ObjectPool<Connection> _connectionPool;
        private readonly ObjectPool<NmsConnection> _nmsconnectionPool;
        private bool _isAlive = true;

        public ActiveMQConsumer(ObjectPool<Connection> connectionPool,
            ObjectPool<NmsConnection> nmsconnectionPool)
        {
            _connectionPool = connectionPool;
            _nmsconnectionPool = nmsconnectionPool;
        }
        public void Dispose()
        {
            _isAlive = false;
        }

        public async Task ReceiveAsync(string queueName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback)
        {
            var connection = _connectionPool.GetAlive();
            try
            {
                using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = await session.GetQueueAsync(queueName);
                    using (IMessageConsumer consumer = await session.CreateConsumerAsync(destination))
                    {
                        while (_isAlive)
                        {
                            IMessage message = await consumer.ReceiveAsync();
                            await message.AcknowledgeAsync();
                            if (message is ITextMessage textMessage)
                            {
                                var result = await callback(textMessage.Text);
                                if (result.NeedReply)
                                {
                                    try
                                    {
                                        IDestination replyDestination = message.NMSReplyTo;
                                        if (replyDestination != null)
                                        {
                                            ITextMessage response = await session.CreateTextMessageAsync(result.ReplyMessage);
                                            response.NMSCorrelationID = message.NMSCorrelationID;
                                            using (IMessageProducer producer = await session.CreateProducerAsync(replyDestination))
                                            {
                                                await producer.SendAsync(response);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error occurred: " + ex.Message);
                                    }
                                    finally
                                    {
                                        Console.WriteLine("need reply: " + result.ReplyMessage);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("don't need reply: " + result.ReplyMessage);
                                }
                            }
                            else if (message == null)
                            {
                                break;
                            }
                        }
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

        public async Task ReceiveMulticastAsync(string topicName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback)
        {
            var connection = _nmsconnectionPool.GetAlive();
            try
            {
                await connection.StartAsync();
                using (ISession session = await connection.CreateSessionAsync(AcknowledgementMode.AutoAcknowledge))
                {
                    IDestination destination = await session.GetTopicAsync($"topic://{topicName}");
                    using (IMessageConsumer consumer = await session.CreateConsumerAsync(destination))
                    {
                        while (_isAlive)
                        {
                            IMessage message = await consumer.ReceiveAsync();
                            await message.AcknowledgeAsync();
                            if (message is ITextMessage textMessage)
                            {
                                var result = await callback(textMessage.Text);
                                Console.WriteLine("Received message: " + textMessage.Text);
                            }
                            else if (message == null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _nmsconnectionPool.ReturnDead(connection);
            }
        }
    }
}
