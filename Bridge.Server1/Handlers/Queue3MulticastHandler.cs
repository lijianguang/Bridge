using Bridge.Server1.Models;
namespace Bridge.Server1.Handlers
{
    [MQHandler(MQType.ActiveMQ, "queue3", true)]
    public class Queue3MulticastHandler : MQHandlerBase
    {
        [MQAction("Test1")]
        public void Test1(MsgTmp msg)
        {
            Thread.Sleep(new Random().Next(10, 200));
        }
    }
}
