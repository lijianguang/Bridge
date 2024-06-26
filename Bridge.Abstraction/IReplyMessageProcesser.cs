namespace Bridge
{
    public interface IReplyMessageProcesser
    {
        T? Process<T>(string replyMessage);
    }
}
