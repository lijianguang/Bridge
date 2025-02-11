namespace Bridge
{
    public abstract class MQHandlerBase : IMQHandler
    {
        public MQContext Context { get; internal set; }
    }
}
