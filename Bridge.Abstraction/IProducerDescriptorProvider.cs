namespace Bridge
{
    public interface IProducerDescriptorProvider
    {
        ProducerDescriptor Get(MQType mqType);
    }
}
