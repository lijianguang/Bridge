namespace Bridge
{
    public class MQContext
    {
        private readonly MQRequest _request;
        private readonly MQResponse _response;
        private readonly string _message;

        public MQContext(MQType mqType, string queueName, string message) 
        {
            _request = new MQRequest(this, mqType, queueName);
            _response = new MQResponse(this);
            _message = message;
        }

        public MQRequest Request
        {
            get { return _request; }
        }

        public MQResponse Response
        {
            get { return _response; }
        }

        public string? Message { get { return _message; } }

        public MQEndpoint? Endpoint { get; internal set; }
        public IServiceProvider? RequestServices { get; internal set; }
    }
}
