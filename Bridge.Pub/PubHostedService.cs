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

            var threads = new List<Thread>();

            var retfor5 = queue5HandlerProxy.Action1Async(new MsgTmp { Name = "A", Age = 1 }).Result;


            for (int i = 0; i < 100; i++)
            {
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test1Async(new MsgTmp { Name = "A", Age = 1 });
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test2Async(new List<MsgTmp>() { new MsgTmp { Name = "A", Age = 1 } });
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test3Async(new Sub1.LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage.SalesOrderHeader());
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test4Async();
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test5Async(5);
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test6Async(12);
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test7Async("name");
                }));
                threads.Add(new Thread(async () =>
                {
                    await queue1HandlerProxy.Test8Async("name");
                }));
            }

            for (int i = 0; i < 100; i++)
            {
                threads.Add(new Thread(async () =>
                {
                    var ret = await queue2HandlerProxy.Test1Async(new MsgTmp { Name = "A1", Age = 1 });

                }));
                threads.Add(new Thread(() =>
                {
                    var ret = queue1HandlerProxy.Test1Async(new MsgTmp { Name = "A", Age = 1 }).Result;
                }));
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    threads.Add(new Thread(() =>
            //    {
            //        queue3HandlerProxy.Test1Async(new MsgTmp { Name = "A", Age = 1 }).Wait();
            //    }));

            //    threads.Add(new Thread(() =>
            //    {
            //        queue3HandlerProxy.Test1Async(new MsgTmp { Name = "A", Age = 1 }).Wait();
            //    }));
            //}

            var timer = new Stopwatch();
            timer.Start();

            threads.ForEach(t =>
            {
                Thread.Sleep(new Random().Next(1, 100));
                t.Start();
            });

            checkAlive:
            var existAlive = false;
            threads.ForEach(t =>
            {
                if (t.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    existAlive = true;
                }
            });
            if (existAlive)
            {
                goto checkAlive;
            }

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceProvider.ReleaseActiveMQResource();
            return Task.CompletedTask;
        }
    }
}
