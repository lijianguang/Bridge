namespace Bridge
{
    public interface IHandlerActionInvoker
    {
        Task<object?> InvokeAsync();
    }
}
