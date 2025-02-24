using Bridge.Server1.Models;
namespace Bridge.Server1.Handlers
{
    [MQHandler(MQType.RabbitMQ, "queue5")]
    public class Queue5Handler : Server1MQHandlerBase
    {
        [MQAction("Action1")]
        public IEnumerable<MsgTmp> Action1(MsgTmp msg)
        {
            var token = Token;
            var marketId = MarketId;

            msg.Name = "Action1";
            msg.Age = 99;
            return new List<MsgTmp>() { msg, msg };
        }
        [MQAction("Action2")]
        public string Action2(string name)
        {
            return $"parameter is {name}";
        }
        [MQAction("Action3")]
        public int Action3(int age)
        {
            return age + 1;
        }
        [MQAction("Action4")]
        public bool Action4(bool isT)
        {
            return !isT;
        }
        [MQAction("Action5")]
        public DateTime Action5(DateTime dt)
        {
            return dt.AddDays(1);
        }

        [MQAction("Action6")]
        public string? Action6(string? name)
        {
            return $"parameter is {name}";
        }
        [MQAction("Action7")]
        public int? Action7(int? age)
        {
            return age + 1;
        }
        [MQAction("Action8")]
        public bool? Action8(bool? isT)
        {
            return !isT;
        }
        [MQAction("Action9")]
        public DateTime? Action9(DateTime? dt)
        {
            return dt?.AddDays(1);
        }
        [MQAction("Action10")]
        public TEST<string, MsgTmp> Action10(TEST<string, MsgTmp> test)
        {
            return test;
        }
        [MQAction("Action11")]
        public TEST<strct, MsgTmp> Action11(TEST<strct, MsgTmp> test)
        {
            return test;
        }
        [MQAction("Action12")]
        public strct Action12(strct test)
        {
            return test;
        }
        [MQAction("Action13")]
        public strct Action13(strct test)
        {
            return test;
        }
        [MQAction("Action14")]
        public void Action14()
        {
            throw new Exception("action14 throw");
        }
        [MQAction("Action15")]
        public string Action15(string name)
        {
            return name.Trim();
        }
    }
}
