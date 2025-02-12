using Bridge.ActiveMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sub1;
using Sub1.Bridge.Message;
using System.Diagnostics;

namespace Bridge.Pub
{
    public class PubHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public PubHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
           
            var queue1HandlerProxy = _serviceProvider.GetRequiredService<Queue1HandlerProxy>();
            var queue2HandlerProxy = _serviceProvider.GetRequiredService<Queue2HandlerProxy>();
            var queue3HandlerProxy = _serviceProvider.GetRequiredService<Queue3HandlerProxy>();
            var queue4HandlerProxy = _serviceProvider.GetRequiredService<Queue4HandlerProxy>();
            var queue5HandlerProxy = _serviceProvider.GetRequiredService<Queue5HandlerProxy>();

            var retfor5 = queue5HandlerProxy.Action1Async(new MsgTmp { Name = "A", Age = 1 }).Result;
            var retforaction2 = queue5HandlerProxy.Action2Async("action2").Result;
            var retforaction3 = queue5HandlerProxy.Action3Async(5).Result;
            var retforaction4 = queue5HandlerProxy.Action4Async(true).Result;
            var retforaction5 = queue5HandlerProxy.Action5Async(DateTime.Now).Result;
            var retforaction6 = queue5HandlerProxy.Action6Async("action6").Result;
            var retforaction61 = queue5HandlerProxy.Action6Async(null).Result;
            var retforaction7 = queue5HandlerProxy.Action7Async(5).Result;
            var retforaction71 = queue5HandlerProxy.Action7Async(null).Result;
            var retforaction8 = queue5HandlerProxy.Action8Async(true).Result;
            var retforaction81 = queue5HandlerProxy.Action8Async(null).Result;
            var retforaction9 = queue5HandlerProxy.Action9Async(DateTime.Now).Result;
            var retforaction91 = queue5HandlerProxy.Action9Async(null).Result;
            var retforaction10 = queue5HandlerProxy.Action10Async(new TEST<string, MsgTmp>
            {
                t = "10",
                t2 = new MsgTmp { Age =10}
            }).Result;
            var retforaction101 = queue5HandlerProxy.Action10Async(null).Result;
            var retforaction11 = queue5HandlerProxy.Action11Async(new TEST<strct, MsgTmp>
            {
                t = new strct { name="11"},
                t2 = new MsgTmp { Age = 10 }
            }).Result;
            var retforaction111 = queue5HandlerProxy.Action11Async(null).Result;
            var retforaction12 = queue5HandlerProxy.Action12Async(new strct
            { 
                name = "12" 
            }).Result;

            _ = queue5HandlerProxy.Action13Async(new strct { });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            return Task.CompletedTask;
        }
    }
}
