using Bridge.Message;

namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, MQNames.Queue3, true)]
    public class Queue3Handler : MQHandlerBase
    {
        [MQAction("Test1")]
        public void Test1(MsgTmp msg)
        {

        }
    }
}
