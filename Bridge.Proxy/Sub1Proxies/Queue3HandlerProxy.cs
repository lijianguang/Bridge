//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sub1 {
    
    
    public class Queue3HandlerProxy : global::Bridge.Abstraction.IHandlerProxy {
        
        private global::Bridge.IPublisher _publisher;
        
        private global::Bridge.MQType _mqType;
        
        public Queue3HandlerProxy(global::Bridge.IPublisher publisher) {
            _publisher = publisher;
            _mqType = global::Bridge.MQType.ActiveMQ;
        }
        
        public async Task Test1Async(Bridge.Message.MsgTmp msg) {
            await _publisher.PublishMulticastAsync(_mqType, "queue3", "Test1", msg);
        }
    }
}
