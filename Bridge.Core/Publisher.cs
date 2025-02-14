namespace Bridge.Core
{
    public class Publisher : IPublisher
    {
        private readonly IProducerFactory _mqProducerFactory;
        private readonly IMessageConverter _messageConverter;
        private readonly IReplyMessageProcesser _replyMessageProcesser;
        private event Action<RequestBody>? _onBefore;
        private event Action<ResponseBody>? _onAfter;

        public Publisher(IProducerFactory mqProducerFactory, IMessageConverter messageConverter, IReplyMessageProcesser replyMessageProcesser)
        {
            _mqProducerFactory = mqProducerFactory;
            _messageConverter = messageConverter;
            _replyMessageProcesser = replyMessageProcesser;
        }

        public async Task PublishAsync<T>(MQType mqType, string queueName, string actionName, T? data, bool needReply = false)
        {
            var requestBody = new RequestBody { ActionName = actionName, NeedReply = needReply, Payload = data };
            if(_onBefore != null)
            {
                _onBefore(requestBody);
            }
            if (needReply)
            {
                var replyMessage = await _mqProducerFactory.Create(mqType)
                    .SendAndWaitReplyAsync(queueName, _messageConverter.Serialize(requestBody));
                var responseBody = _messageConverter.Deserialize<ResponseBody>(replyMessage);
                if (_onAfter != null)
                {
                    _onAfter(responseBody);
                }
                _replyMessageProcesser.Process<object>(responseBody);
            }
            else
            {
                await _mqProducerFactory.Create(mqType)
                    .SendAsync(queueName, _messageConverter.Serialize(requestBody));
            }
        }

        public async Task PublishAsync(MQType mqType, string queueName, string actionName, bool needReply = false)
        {
            await PublishAsync(mqType, queueName, actionName, default(object), needReply);
        }

        public async Task<TR?> PublishAndWaitReplyAsync<T, TR>(MQType mqType, string queueName, string actionName, T? data)
        {
            var requestBody = new RequestBody { ActionName = actionName, NeedReply = true, Payload = data };
            if (_onBefore != null)
            {
                _onBefore(requestBody);
            }
            var replyMessage = await _mqProducerFactory.Create(mqType)
                .SendAndWaitReplyAsync(queueName, _messageConverter.Serialize(requestBody));
            var responseBody = _messageConverter.Deserialize<ResponseBody>(replyMessage);
            if (_onAfter != null)
            {
                _onAfter(responseBody);
            }
            return _replyMessageProcesser.Process<TR>(responseBody);
        }

        public async Task<TR?> PublishAndWaitReplyAsync<TR>(MQType mqType, string queueName, string actionName) 
        {
            return await PublishAndWaitReplyAsync<object, TR>(mqType, queueName, actionName, default(object));
        }

        public async Task PublishMulticastAsync<T>(MQType mqType, string queueName, string actionName, T? data)
        {
            var requestBody = new RequestBody { ActionName = actionName, NeedReply = false, Payload = data };
            if (_onBefore != null)
            {
                _onBefore(requestBody);
            }
            await _mqProducerFactory.Create(mqType)
                .SendMulticastAsync(queueName, _messageConverter.Serialize(requestBody));
        }

        public async Task PublishMulticastAsync(MQType mqType, string queueName, string actionName)
        {
            await PublishMulticastAsync(mqType, queueName, actionName, default(object));
        }

        public void OnBefore(Action<RequestBody> onRequest)
        {
            _onBefore += onRequest;
        }

        public void OnAfter(Action<ResponseBody> onResponse)
        {
            _onAfter += onResponse;
        }
    }
}
