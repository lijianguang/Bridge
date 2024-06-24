﻿using Bridge.Message;

namespace Bridge.Sub
{
    [MQHandler(MQType.ActiveMQ, MQNames.Queue2)]
    public class Queue2Handler : MQHandlerBase
    {
        public Queue2Handler() { }

        [MQAction("Test1")]
        public async Task<MsgTmp> Test1(MsgTmp msg)
        {
            throw new NotImplementedException("hello world.");
            return await Task.Run(() =>
            {
                msg.Name = "updated";
                msg.Age = 19;
                return msg;
            });
        }

        [MQAction("Test2")]
        public void Test2(MsgTmp msg)
        {

        }

        [MQAction("Test3")]
        public void Test3()
        {

        }

        [MQAction("Test4")]
        public MsgTmp Test4()
        {
            return new() { Name = "new", Age = 101 };
        }

        [MQAction("Test5")]
        public MsgTmp Test5(int age)
        {
            return new() { Name = "new", Age = age };
        }

        [MQAction("Test6")]
        public MsgTmp Test6(int? age)
        {
            return new() { Name = "new", Age = age.HasValue ? age.Value : 0 };
        }

        [MQAction("Test7")]
        public MsgTmp Test7(string name)
        {
            return new() { Name = name, Age = 7 };
        }

        [MQAction("Test8")]
        public MsgTmp Test8(string? name)
        {
            return new() { Name = name, Age = 8 };
        }
    }
}
