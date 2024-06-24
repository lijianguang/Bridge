namespace Bridge
{
    public interface IMQHandlerFactory
    {
        IMQHandler Create(Type mqHandlerType);
    }
}
