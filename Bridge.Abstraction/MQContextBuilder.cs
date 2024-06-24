namespace Bridge
{
    public class MQContextBuilder
    {
        private readonly MQContext _mqContext;

        public MQContextBuilder(MQType mqType, string queueName)
        {
            _mqContext = new MQContext(mqType, queueName);
        }

        public void Configure(Action<MQContext> configureContext)
        {
            configureContext(_mqContext);
        }
        public MQContext Build()
        {
            return _mqContext;
        }
    }
}
