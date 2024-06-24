using Bridge;
using Bridge.Message;
using Bridge.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bridge.ActiveMQ;
using Bridge.Sub;
using System.Diagnostics;
using Microsoft.Extensions.ObjectPool;
using Apache.NMS.ActiveMQ;
using System;
using Apache.NMS;

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

                    services.AddTransient<Queue1Handler>();
                    services.AddTransient<Queue2Handler>();

                });
            var host = builder.Build();
            _serviceProvider = host.Services;
        }

        [Test]
        public void Test1()
        {
            var tasks = new List<Task>();
            var timer = new Stopwatch();
            timer.Start();

            var connectionPool = _serviceProvider.GetRequiredService<ObjectPool<Connection>>();
            var nmsconnectionPool = _serviceProvider.GetRequiredService<ObjectPool<Apache.NMS.AMQP.NmsConnection>>();

            for (int i = 0;i < 100; i++)
            {
                Thread t = new Thread(() =>
                {
                    var connection = connectionPool.GetAlive();
                    connectionPool.ReturnDead(connection);
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.ThreadState 
                //tasks.Add(Task.Run(() =>
                //{
                //    for(int j = 0; j < 1; j++)
                //    {
                //        //IConnectionFactory factory = new ConnectionFactory("failover:(tcp://127.0.0.1:61616)");
                //        //var connection = (Connection)factory.CreateConnection("admin", "admin");
                //        //connection.Start();
                //        ////using (ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge))
                //        ////{
                //        ////    IDestination destination = session.GetQueue("queue1");
                //        ////    using (IMessageProducer producer = session.CreateProducer(destination))
                //        ////    {
                //        ////        ITextMessage textMessage = producer.CreateTextMessage("test");
                //        ////        producer.Send(textMessage);
                //        ////    }
                //        ////}
                //        //connection.Stop();
                //        //connection.Dispose();

                //        var connection = connectionPool.GetAlive();
                //        connectionPool.ReturnDead(connection);
                //    }
                //}
                //));
            }

            Task.WhenAll(tasks).Wait();
            TimeSpan timeTaken = timer.Elapsed;
            string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
            Console.WriteLine(foo);
            return;
            var publisher = _serviceProvider.GetRequiredService<IPublisher>();
            //var tasks = new List<Task>();

            //var result = publisher.PublishAndWaitReplyAsync<MsgTmp, MsgTmp>(MQType.ActiveMQ, MQNames.Queue2, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;

            //return;
            for (int i = 0; i < 1; i++)
            {
                //tasks.Add(Task.Run(() =>
                //{
                //    var result = publisher.PublishAndWaitReplyAsync<MsgTmp, MsgTmp>(MQType.ActiveMQ, MQNames.Queue2, "Test1", new MsgTmp { Name = "A", Age = 1 }).Result;

                //}));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue2, "Test2", new MsgTmp { Name = "A", Age = 1 });
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test3");
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test4");
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test5", 5);
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test6");
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test7", 5);
                }));
                tasks.Add(Task.Run(() =>
                {
                    publisher.PublishAsync(MQType.ActiveMQ, MQNames.Queue1, "Test8", 9);
                }));
            }

            Task.WhenAll(tasks).Wait();

            Thread.Sleep(100000);
        }
    }
}