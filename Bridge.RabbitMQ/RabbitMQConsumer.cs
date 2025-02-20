using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace Bridge.RabbitMQ
{
    [ConsumerMapping(MQType.RabbitMQ)]
    public class RabbitMQConsumer : IConsumer
    {
        private readonly ObjectPool<IChannel> _channelPool;
        private bool _isAlive = true;

        public RabbitMQConsumer(ObjectPool<IChannel> channelPool)
        {
            _channelPool = channelPool;
        }
        public void Dispose()
        {
            _isAlive = false;
        }

        public async Task ReceiveAsync(string queueName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback)
        {
            var channel = _channelPool.GetAliveChannel();
            try
            {
                while(_isAlive)
                {
                    await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
                    var data = await channel.BasicGetAsync(queueName, true);
                    if(data != null)
                    {
                        var message = System.Text.Encoding.UTF8.GetString(data.Body.ToArray());
                        IReadOnlyBasicProperties props = data.BasicProperties;
                        var replyProps = new BasicProperties
                        {
                            CorrelationId = props.CorrelationId
                        };
                        
                        var result = await callback(message);
                        if (result.NeedReply)
                        {
                            var replyBody = System.Text.Encoding.UTF8.GetBytes(result.ReplyMessage);
                            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: props.ReplyTo!,
                                    mandatory: true, basicProperties: replyProps, body: replyBody);
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
                _channelPool.ReturnSuspendedChannel(channel);
            }
        }

        public async Task ReceiveMulticastAsync(string topicName, Func<string, Task<(bool NeedReply, string ReplyMessage)>> callback)
        {
            var channel = _channelPool.GetAliveChannel();
            try
            {
                while (_isAlive)
                {
                    await channel.ExchangeDeclareAsync(exchange: topicName, type: ExchangeType.Fanout);

                    QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
                    string queueName = queueDeclareResult.QueueName;
                    await channel.QueueBindAsync(queue: queueName, exchange: topicName, routingKey: string.Empty);

                    var data = await channel.BasicGetAsync(queueName, true);
                    if (data != null)
                    {
                        var message = System.Text.Encoding.UTF8.GetString(data.Body.ToArray());
                        var result = callback(message);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
            }
            finally
            {
                _channelPool.ReturnSuspendedChannel(channel);
            }
        }
    }
}
