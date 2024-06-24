namespace Bridge
{
    public interface IProducerFactory
    {
        IProducer Create(MQType mqType);
    }
}
