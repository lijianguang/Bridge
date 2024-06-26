namespace Bridge
{
    public class MQContextBuilder
    {
        private readonly MQContext _mqContext;

        public MQContextBuilder(MQType mqType, string queueName, string message)
        {
            _mqContext = new MQContext(mqType, queueName, message);
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
