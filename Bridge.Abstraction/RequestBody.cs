namespace Bridge
{
    public sealed class RequestBody
    {
        public required string ActionName { get; set; }
        public required bool NeedReply { get; set; }
        public object? Payload { get; set; }
    }
}
