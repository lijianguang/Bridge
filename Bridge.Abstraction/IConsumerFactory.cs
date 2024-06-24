namespace Bridge
{
    public interface IConsumerFactory
    {
        IConsumer Create(MQType mqType);
    }
}
