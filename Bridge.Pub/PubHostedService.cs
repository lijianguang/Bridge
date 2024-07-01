using Bridge.ActiveMQ;
using Bridge.Message;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var threads = new List<Thread>();
            foreach (var q in new[] { MQNames.Queue1, MQNames.Queue2 })
            {
                for (int i = 0; i < 100; i++)
                {
                    threads.Add(new Thread(async () =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        await publisher.ActiveMQ_PublishAsync(q, "Test2", new List<MsgTmp>() { new MsgTmp { Name = "A", Age = 1 } });
                    }));
                    threads.Add(new Thread(async () =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        await publisher.ActiveMQ_PublishAsync(q, "Test2", new List<MsgTmp>() { new MsgTmp { Name = "A", Age = 1 } });
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test3").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test3").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test4").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test4").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test5", 5).Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test5", 5).Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test6").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test6").Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test7", 5).Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test7", 5).Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test8", 9).Wait();
                    }));
                    threads.Add(new Thread(() =>
                    {
                        var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                        publisher.ActiveMQ_PublishAsync(q, "Test8", 9).Wait();
                    }));
                }
            }

            for (int i = 0; i < 100; i++)
            {
                threads.Add(new Thread(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    var result1 = publisher.ActiveMQ_PublishAndWaitReplyAsync<MsgTmp, MsgTmp>(MQNames.Queue2, "Test1", new MsgTmp { Name = "A1", Age = 1 }).Result!;
                }));
                threads.Add(new Thread(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    var result = publisher.ActiveMQ_PublishAndWaitReplyAsync<MsgTmp, IEnumerable<MsgTmp>>(MQNames.Queue1, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;
                }));
            }

            for (int i = 0; i < 100; i++)
            {
                threads.Add(new Thread(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    publisher.ActiveMQ_PublishMulticastAsync(MQNames.Queue3, "Test1", new MsgTmp { Name = "A", Age = 1 }).Wait();
                }));

                threads.Add(new Thread(() =>
                {
                    var publisher = _serviceProvider.GetRequiredService<IPublisher>();
                    publisher.ActiveMQ_PublishMulticastAsync(MQNames.Queue4, "Test1", new MsgTmp { Name = "A", Age = 1 }).Wait();
                }));
            }

            var timer = new Stopwatch();
            timer.Start();

            threads.ForEach(t =>
            {
                Thread.Sleep(new Random().Next(1, 10));
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
