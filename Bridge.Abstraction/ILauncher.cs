namespace Bridge
{
    public interface ILauncher
    {
        Task LaunchAsync(MQType mqType, string queueName, bool isMulticast, MQDelegate pipelineEntry);
        Task LaunchAsync(MQType mqType, MQDelegate pipelineEntry);
        Task LaunchAsync(MQDelegate pipelineEntry);
        Task StopAsync();
    }
}
