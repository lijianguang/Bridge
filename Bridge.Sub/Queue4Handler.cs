using Bridge.Message;

namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, MQNames.Queue4, true)]
    public class Queue4Handler : MQHandlerBase
    {
        [MQAction("Test1")]
        public async Task Test1(MsgTmp msg)
        {
        }

        [MQAction("Test2")]
        public async Task<int> Test2(MsgTmp msg)
        {
            return 1;
        }

        [MQAction("Test3")]
        public async Task<bool> Test3(IEnumerable<MsgTmp> msgs)
        {
            return true;
        }

        [MQAction("Test4")]
        public async Task<IEnumerable<MsgTmp>> Test4(int i)
        {
            return default(IEnumerable<MsgTmp>);
        }
    }
}
