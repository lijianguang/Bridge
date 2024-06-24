﻿using Newtonsoft.Json.Linq;

namespace Bridge.Core
{
    public class Publisher : IPublisher
    {
        private readonly IProducerFactory _mqProducerFactory;
        private readonly IMessageConverter _messageConverter;

        public Publisher(IProducerFactory mqProducerFactory, IMessageConverter messageConverter)
        {
            _mqProducerFactory = mqProducerFactory;
            _messageConverter = messageConverter;
        }

        public async Task PublishAsync<T>(MQType mqType, string queueName, string actionName, T? data)
        {
            await _mqProducerFactory.Create(mqType)
                .SendAsync(queueName, _messageConverter.Serialize(new RequestBody { ActionName = actionName, NeedReply = false, Payload = data }));
        }

        public async Task PublishAsync(MQType mqType, string queueName, string actionName)
        {
            await PublishAsync(mqType, queueName, actionName, default(object));
        }

        public async Task<TR?> PublishAndWaitReplyAsync<T, TR>(MQType mqType, string queueName, string actionName, T? data)
        {
            var replyMessage = await _mqProducerFactory.Create(mqType)
                .SendAndWaitReplyAsync(queueName, _messageConverter.Serialize(new RequestBody { ActionName = actionName, NeedReply = true, Payload = data }));

            var responseBody = _messageConverter.Deserialize<ResponseBody>(replyMessage);

            if(responseBody.StatusCode == MQStatusCode.InternalServerError)
            {
                throw responseBody.Payload is JToken jtokenException ? jtokenException.ToObject<Exception>()! : new Exception("Unknown exception.");
            }

            return responseBody.Payload is JToken jtoken ? jtoken.ToObject<TR>() : default;
        }

        public async Task<TR?> PublishAndWaitReplyAsync<TR>(MQType mqType, string queueName, string actionName) 
        {
            return await PublishAndWaitReplyAsync<object, TR>(mqType, queueName, actionName, default(object));
        }

        public async Task PublishMulticastAsync<T>(MQType mqType, string queueName, string actionName, T? data)
        {
            await _mqProducerFactory.Create(mqType)
                .SendMulticastAsync(queueName, _messageConverter.Serialize(new RequestBody { ActionName = actionName, NeedReply = false, Payload = data }));
        }

        public async Task PublishMulticastAsync(MQType mqType, string queueName, string actionName)
        {
            await PublishMulticastAsync(mqType, queueName, actionName, default(object));
        }
    }
}
