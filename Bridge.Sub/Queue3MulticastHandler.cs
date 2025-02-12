using Bridge.Sub.Models;
namespace Bridge.Sub
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
