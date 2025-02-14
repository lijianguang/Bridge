using Bridge.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bridge.ActiveMQ;
using System.Diagnostics;
using Microsoft.Extensions.ObjectPool;
using Apache.NMS.ActiveMQ;
using Apache.NMS;
using Bridge.Sub.Handlers;

namespace UnitTest.Bridge
{
    public class Tests
    {
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            var builder = Host.CreateDefaultBuilder([])
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<HostOptions>().Configure(
                           opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(60));

                    services.AddBridgeServices();

                    services.AddActiveMQServices(hostContext.Configuration);

                    services.AddAllHandlerProxies();

                });
            var host = builder.Build();
            _serviceProvider = host.Services;
        }

        [Test]
        public void Test1()
        {
            var tasks = new List<Task>();
            var threads = new List<Thread>();
            var timer = new Stopwatch();
            timer.Start();

            var connectionPool = _serviceProvider.GetRequiredService<ObjectPool<Connection>>();
            var nmsconnectionPool = _serviceProvider.GetRequiredService<ObjectPool<Apache.NMS.AMQP.NmsConnection>>();

            for (int i = 0;i < 100; i++)
            {
                threads.Add(new Thread(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        //var connection = connectionPool.GetAlive();
                        //connectionPool.ReturnDead(connection);

                        IConnectionFactory factory = new ConnectionFactory("failover:(tcp://127.0.0.1:61616)");
                        var connection = (Connection)factory.CreateConnection("admin", "admin");
                        connection.Start();

                        connection.Stop();
                        connection.Dispose();
                    }
                }));
            }

            threads.ForEach(t =>
            {
                t.Start();
            });
            checkAlive:
            var existAlive = false;
            threads.ForEach(t =>
            {
                if(t.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    existAlive = true;
                }
            });
            if (existAlive)
            {
                goto checkAlive;
            }

            Task.WhenAll(tasks).Wait();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
        }
    }
}