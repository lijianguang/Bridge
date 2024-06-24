namespace Bridge
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConsumerMappingAttribute : Attribute
    {
        private MQType _mqType;

        public ConsumerMappingAttribute(MQType mqType)
        {
            if (mqType == MQType.Unknown) throw new ArgumentException("mqType must not be unknown.");

            _mqType = mqType;
        }

        public MQType MQType { get { return _mqType; } }
    }
}
