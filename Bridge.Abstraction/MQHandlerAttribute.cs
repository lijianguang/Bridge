namespace Bridge
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class MQHandlerAttribute : Attribute
    {
        private readonly MQType _mqType;
        private readonly string _queueName;
        private readonly bool _isMulticast;

        public MQHandlerAttribute(MQType mqType, string queueName, bool isMulticast = false)
        {
            if (mqType == MQType.Unknown) throw new ArgumentException("mqType must not be unknown.");
            if (string.IsNullOrEmpty(queueName)) throw new ArgumentException("queueName must not be null or empty.");

            _mqType = mqType;
            _queueName = queueName;
            _isMulticast = isMulticast;
        }

        public MQType MQType { get { return _mqType; } }
        public string QueueName { get { return _queueName; } }
        public bool IsMulticast { get { return _isMulticast; } }
    }
}
