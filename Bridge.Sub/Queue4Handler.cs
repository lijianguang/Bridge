using Bridge.Message;

namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, MQNames.Queue4, true)]
    public class Queue4Handler : MQHandlerBase
    {
        [MQAction("Test1")]
        public void Test1(MsgTmp msg)
        {
            Thread.Sleep(new Random().Next(10, 200));
        }
    }
}
