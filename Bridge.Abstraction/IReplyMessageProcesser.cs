namespace Bridge
{
    public interface IReplyMessageProcesser
    {
        T? Process<T>(ResponseBody responseBody);
    }
}
