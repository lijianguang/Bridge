﻿using Bridge.Message;

namespace Bridge.Sub1
{
    [MQHandler(MQType.ActiveMQ, MQNames.Topic1, true)]
    public class Topic1Handler : MQHandlerBase
    {
        [MQAction("Test1")]
        public void Test1(MsgTmp msg)
        {

        }
    }
}
