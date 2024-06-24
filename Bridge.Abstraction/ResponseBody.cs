namespace Bridge
{
    public class ResponseBody
    {
        public MQStatusCode StatusCode { get; set; }
        public object? Payload { get; set; }
    }
}
