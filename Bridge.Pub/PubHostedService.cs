using Bridge.ActiveMQ;
using Bridge.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
            var timer = new Stopwatch();
            timer.Start();

            var tasks = new List<Task>();
            //foreach (var q in new[] { MQNames.Queue1, MQNames.Queue2 })
            //{
            //    Thread.Sleep(new Random().Next(900, 1000));
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Thread.Sleep(new Random().Next(900, 1000));
            //        //tasks.Add(Task.Run(() =>
            //        //{
            //        //    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //        //    var result = publisher.PublishAndWaitReplyAsync<MsgTmp, MsgTmp>(MQType.ActiveMQ, q, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;

            //        //}));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test2", new MsgTmp { Name = "A", Age = 1 }).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test2", new MsgTmp { Name = "A", Age = 1 }).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test3").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test3").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test4").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test4").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test5", 5).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test5", 5).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test6").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test6").Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test7", 5).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test7", 5).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test8", 9).Wait();
            //        }));
            //        tasks.Add(Task.Run(() =>
            //        {
            //            Thread.Sleep(new Random().Next(900, 1000));
            //            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //            publisher.PublishAsync(MQType.ActiveMQ, q, "Test8", 9).Wait();
            //        }));
            //    }
            //}

            for (int i = 0; i < 1; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    var result = publisher.PublishAndWaitReplyAsync<MsgTmp, MsgTmp>(MQType.ActiveMQ, MQNames.Queue2, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;
                }));
                tasks.Add(Task.Run(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    var result = publisher.PublishAndWaitReplyAsync<MsgTmp, IEnumerable<MsgTmp>>(MQType.ActiveMQ, MQNames.Queue1, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;
                }));

            }

            //for (int i = 0; i < 10; i++)
            //{
            //    tasks.Add(Task.Run(() =>
            //    {
            //        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //        publisher.PublishMulticastAsync(MQType.ActiveMQ, MQNames.Topic1, "Test1", new MsgTmp { Name = "A", Age = 1 }).Wait();
            //    }));

            //    tasks.Add(Task.Run(() =>
            //    {
            //        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //        publisher.PublishMulticastAsync(MQType.ActiveMQ, MQNames.Topic2, "Test1", new MsgTmp { Name = "A", Age = 1 }).Wait();
            //    }));
            //}
            Task.WhenAll(tasks).Wait();

            //B: Run stuff you want timed
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
