namespace Bridge
{
    public class ConsumerDescriptor
    {
        public MQType MQType { get; set; }
        public required Type Type { get; set; }
    }
}
