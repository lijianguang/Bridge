namespace Bridge
{
    public interface IConsumerDescriptorProvider
    {
        ConsumerDescriptor Get(MQType mqType);
    }
}
