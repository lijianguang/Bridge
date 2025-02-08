namespace Bridge
{
    public sealed class RequestBody
    {
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
        public required string ActionName { get; set; }
        public required bool NeedReply { get; set; }
        public object? Payload { get; set; }
    }
}
