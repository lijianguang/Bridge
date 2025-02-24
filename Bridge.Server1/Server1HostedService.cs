using Bridge.ActiveMQ;
using Bridge.Core;
using Bridge.RabbitMQ;
using Microsoft.Extensions.Hosting;

namespace Bridge.Server1
{
    public class Server1HostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILauncher _launcher;
        private readonly IMQPipelineBuilder _mqPipelineBuilder;

        public Server1HostedService(IServiceProvider serviceProvider, ILauncher launcher, IMQPipelineBuilder mqPipelineBuilder) 
        {
            _serviceProvider = serviceProvider;
            _launcher = launcher;
            _mqPipelineBuilder = mqPipelineBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var _pipelineEntry = _mqPipelineBuilder
                .UseMiddleware<MQExceptionHandlerMiddleware>()
                .UseMiddleware<MQLogToConsoleMiddleware>()
                .UseMiddleware<MQAnalyseRequestBodyMiddleware>()
                .UseMiddleware<MQEndpointRoutingMiddleware>()
                .UseMiddleware<MQEndpointMiddleware>()
                .Build();

            return _launcher.LaunchAsync(_pipelineEntry);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            _serviceProvider.ReleaseRabbitMQResource();
            return _launcher.StopAsync();
        }
    }
}
