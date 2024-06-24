namespace Bridge
{
    public class MQRequest
    {
        private readonly MQContext _context;
        private readonly MQType _mqType;
        private readonly string _queueName;

        public MQRequest(MQContext context, MQType mqType, string queueName)
        {
            _context = context;
            _mqType = mqType;
            _queueName = queueName;
        }

        public string? Body {  get; set; }
        public bool NeedReply { get; set; }
        public Func<Type, object?> Payload { get; set; } = (_) => default;
        public MQContext MQContext { get { return _context; } }
        public MQType MQType { get { return _mqType; } }
        public string QueueName { get { return _queueName; } }
        public string? ActionName { get; set; }
    }
}
