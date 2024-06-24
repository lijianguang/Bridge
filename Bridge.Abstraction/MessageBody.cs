namespace Bridge
{
    public sealed class MessageBody
    {
        public required string ActionName { get; set; }
        public required bool NeedReply { get; set; }
        public object? Payload { get; set; }
    }
}
