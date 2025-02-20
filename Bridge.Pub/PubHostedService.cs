using Amqp.Framing;
using Bridge.ActiveMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sub1;
using Sub1.Bridge.Sub.Models;

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
            var queue1HandlerProxy = _serviceProvider.GetRequiredService<ActiveMQ_Queue1_Proxy>();
            var queue3HandlerProxy = _serviceProvider.GetRequiredService<Sub1.ActiveMQ_Queue3Multicast_Proxy>();
            var queue5HandlerProxy = _serviceProvider.GetRequiredService<RabbitMQ_Queue5_Proxy>();

            var ret0 = queue5HandlerProxy.Action2Async("hello").Result;
            var ret = queue5HandlerProxy.Action3Async(18).Result;

            while (true)
            {
                Begin:
                Console.WriteLine("please enter the queue name:");

                var queueName = Console.ReadLine();
                if (string.IsNullOrEmpty(queueName))
                {
                    goto Begin;
                }

                switch (queueName)
                {
                    case "queue1":
                        break;
                    case "queue3":
                        Q3ActionNameBegin:
                        Console.WriteLine($"please enter the {queueName}'s action name:");
                        var q3ActionName = Console.ReadLine();
                        switch (q3ActionName)
                        {
                            case "test1":
                                _ = queue3HandlerProxy.Test1Async(new MsgTmp { });
                                break;
                            default:
                                goto Q3ActionNameBegin;
                        }
                        break;
                    case "queue5":
                        ActionNameBegin:
                        Console.WriteLine($"please enter the {queueName}'s action name:");
                        var actionName = Console.ReadLine();
                        switch (actionName)
                        {
                            case "action1":
                                var queue5Action1Result = queue5HandlerProxy.Action1Async(new MsgTmp { }).Result;
                                break;
                            case "action2":
                                var queue5Action2Result = queue5HandlerProxy.Action2Async("action2").Result;
                                break;
                            case "action3":
                                var queue5Action3Result = queue5HandlerProxy.Action3Async(5).Result;
                                break;
                            case "action4":
                                var queue5Action4Result = queue5HandlerProxy.Action4Async(true).Result;
                                break;
                            case "action5":
                                var queue5Action5Result = queue5HandlerProxy.Action5Async(DateTime.Now).Result;
                                break;
                            case "action6":
                                var queue5Action6Result = queue5HandlerProxy.Action6Async(null).Result;
                                var queue5Action61Result = queue5HandlerProxy.Action6Async("action6").Result;
                                break;
                            case "action7":
                                var queue5Action7Result = queue5HandlerProxy.Action7Async(5).Result;
                                var queue5Action71Result = queue5HandlerProxy.Action7Async(null).Result;
                                break;
                            case "action8":
                                var queue5Action8Result = queue5HandlerProxy.Action8Async(true).Result;
                                var queue5Action8Resul1 = queue5HandlerProxy.Action8Async(null).Result;
                                break;
                            case "action9":
                                var queue5Action9Result = queue5HandlerProxy.Action9Async(DateTime.Now).Result;
                                var queue5Action91Result = queue5HandlerProxy.Action9Async(null).Result;
                                break;
                            case "action10":
                                var queue5Action10Result = queue5HandlerProxy.Action10Async(new Sub1.TEST<string, MsgTmp>
                                {
                                    t = "10",
                                    t2 = new MsgTmp { Age = 10 }
                                }).Result;
                                break;
                            case "action11":
                                var queue5Action11Result = queue5HandlerProxy.Action11Async(new Sub1.TEST<strct, MsgTmp>
                                {
                                    t = new strct { name = "11" },
                                    t2 = new MsgTmp { Age = 10 }
                                }).Result;
                                break;
                            case "action12":
                                var queue5Action12Result = queue5HandlerProxy.Action12Async(new strct
                                {
                                    name = "12"
                                }).Result;
                                break;
                            default:
                                Console.WriteLine($"can't find the {queueName}'s action: {actionName}");
                                goto ActionNameBegin;
                        }
                        break;
                    default:
                        Console.WriteLine($"can't find the queue: {queueName}");
                        goto Begin;
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            return Task.CompletedTask;
        }
    }
}
