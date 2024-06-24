namespace Bridge
{
    public class MQContext
    {
        private readonly MQRequest _request;
        private readonly MQResponse _response;

        public MQContext(MQType mqType, string queueName) 
        {
            _request = new MQRequest(this, mqType, queueName);
            _response = new MQResponse(this);
        }

        public MQRequest Request
        {
            get { return _request; }
        }
        public MQResponse Response
        {
            get { return _response; }
        }

        public MQEndpoint? Endpoint { get; set; }
        public IServiceProvider? RequestServices { get; set; }
    }
}
