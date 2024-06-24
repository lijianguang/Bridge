namespace Bridge
{
    public interface IMQMiddleware
    {
        Task InvokeAsync(MQContext context, MQDelegate next);
    }
}
