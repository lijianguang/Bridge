namespace Bridge
{
    public class MQResponse
    {
        private readonly MQContext _context;

        public MQResponse(MQContext context)
        {
            _context = context;
        }

        public MQContext MQContext { get { return _context; } }
        public bool NeedReply { get; set; }
        public ResponseBody Body { get; set; } = new ResponseBody() { StatusCode = MQStatusCode.BadRequest };
    }
}
