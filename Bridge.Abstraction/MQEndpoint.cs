namespace Bridge
{
    public class MQEndpoint
    {
        public MQDelegate _mqDelegate;
        public MQEndpoint(MQDelegate mqDelegate) 
        {
            _mqDelegate = mqDelegate;
        }
        public MQDelegate MQDelegate { get { return _mqDelegate; } }
    }
}
