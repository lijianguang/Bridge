using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System.Text;

namespace Bridge.RabbitMQ
{
    [ProducerMapping(MQType.RabbitMQ)]
    public class RabbitMQProducer : IProducer
    {
        private readonly ObjectPool<IChannel> _channelPool;
        public RabbitMQProducer(ObjectPool<IChannel> channelPool)
        {
            _channelPool = channelPool;
        }

        public async Task<string> SendAndWaitReplyAsync(string queueName, string message)
        {
            var channel = _channelPool.GetAliveChannel();
            try
            {
                var queueDeclareResult = await channel.QueueDeclareAsync();

                var replyQueueName = queueDeclareResult.QueueName;

                var body = Encoding.UTF8.GetBytes(message);
                string correlationId = Guid.NewGuid().ToString();
                var props = new BasicProperties
                {
                    CorrelationId = correlationId,
                    ReplyTo = replyQueueName
                };
                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName,mandatory: true, basicProperties: props, body: body);

                while (true)
                {
                    Thread.Sleep(100);
                    var data = await channel.BasicGetAsync(replyQueueName, true);
                    if (data != null)
                    {
                        var replyMessage = System.Text.Encoding.UTF8.GetString(data.Body.ToArray());
                        return replyMessage;
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
            return default;
        }

        public async Task SendAsync(string queueName, string message)
        {
            var channel = _channelPool.GetAliveChannel();
            try
            {
                var queueDeclareResult  = await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false,
                arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);
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

        public async Task SendMulticastAsync(string queueName, string message)
        {
            var channel = _channelPool.GetAliveChannel();
            try
            {
                await channel.ExchangeDeclareAsync(exchange: queueName, type: ExchangeType.Fanout);

                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync(exchange: queueName, routingKey: string.Empty, body: body);
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
