using Bridge.Message;
namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, "queue5")]
    public class Queue5Handler : Sub1MQHandlerBase
    {
        [MQAction("Action1")]
        public IEnumerable<MsgTmp> Action1(MsgTmp msg)
        {
            var token = this.Token;
            var marketId = this.MarketId;

            msg.Name = "Action1";
            msg.Age = 99;
            return new List<MsgTmp>() { msg, msg };
        }
    }
}
