using Bridge.ActiveMQ;
using Bridge.Core;
using Microsoft.Extensions.Hosting;

namespace Bridge.Server2
{
    public class Server2HostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILauncher _launcher;
        private readonly IMQPipelineBuilder _mqPipelineBuilder;

        public Server2HostedService(IServiceProvider serviceProvider, ILauncher launcher, IMQPipelineBuilder mqPipelineBuilder) 
        {
            _serviceProvider = serviceProvider;
            _launcher = launcher;
            _mqPipelineBuilder = mqPipelineBuilder;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var _pipelineEntry = _mqPipelineBuilder
                .UseMiddleware<MQExceptionHandlerMiddleware>()
                .UseMiddleware<MQAnalyseRequestBodyMiddleware>()
                .UseMiddleware<MQEndpointRoutingMiddleware>()
                .UseMiddleware<MQEndpointMiddleware>()
                .Build();

            return _launcher.LaunchAsync(_pipelineEntry);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            return _launcher.StopAsync();
        }
    }
}
