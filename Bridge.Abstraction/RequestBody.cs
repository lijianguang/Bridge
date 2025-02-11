namespace Bridge
{
    public sealed class RequestBody
    {
        public IDictionary<string, string> Headers { get; internal set; } = new Dictionary<string, string>();
        public required string ActionName { get; set; }
        public required bool NeedReply { get; set; }
        public object? Payload { get; set; }
    }
}
