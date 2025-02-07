using Bridge.Message;

namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, MQNames.Queue1)]
    public class Queue1Handler : MQHandlerBase
    {
        public Queue1Handler() { }

        [MQAction("Test1")]
        public IEnumerable<MsgTmp> Test1(MsgTmp msg)
        {
            Thread.Sleep(new Random().Next(10, 200));
            msg.Name = "updated";
            msg.Age = 99;
            return new List<MsgTmp>() { msg,msg };
        }

        [MQAction("Test2")]
        public void Test2(IEnumerable<MsgTmp> msgs)
        {
            Thread.Sleep(new Random().Next(10, 200));
        }

        [MQAction("Test3")]
        public void Test3(LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage.SalesOrderHeader so)
        {
            Thread.Sleep(new Random().Next(10, 200));
        }

        [MQAction("Test4")]
        public MsgTmp Test4()
        {
            Thread.Sleep(new Random().Next(10, 200));
            return new() { Name = "new", Age = 101 };
        }

        [MQAction("Test5")]
        public MsgTmp Test5(int age)
        {
            Thread.Sleep(new Random().Next(10, 200));
            return new() { Name = "new", Age = age };
        }

        [MQAction("Test6")]
        public MsgTmp Test6(int? age)
        {
            Thread.Sleep(new Random().Next(10, 200));
            return new() { Name = "new", Age = age.HasValue ? age.Value : 0 };
        }

        [MQAction("Test7")]
        public MsgTmp Test7(string name)
        {
            Thread.Sleep(new Random().Next(10, 200));
            return new() { Name = name, Age = 7 };
        }

        [MQAction("Test8")]
        public MsgTmp Test8(string? name)
        {
            Thread.Sleep(new Random().Next(10, 200));
            return new() { Name = name, Age = 8 };
        }
    }
}
