namespace Bridge
{
    public interface IMQPipelineBuilder
    {
        IMQPipelineBuilder Use(Func<MQDelegate, MQDelegate> process);

        IMQPipelineBuilder UseMiddleware<T>();

        MQDelegate Build();
    }
}
