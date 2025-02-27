//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sub1 {
    
    
    public class Queue2HandlerProxy : global::Bridge.Abstraction.IHandlerProxy {
        
        private global::Bridge.IPublisher _publisher;
        
        private global::Bridge.MQType _mqType;
        
        public Queue2HandlerProxy(global::Bridge.IPublisher publisher) {
            _publisher = publisher;
            _mqType = global::Bridge.MQType.ActiveMQ;
        }
        
        public async Task<Bridge.Message.MsgTmp> Test1Async(Bridge.Message.MsgTmp msg) {
            return await _publisher.PublishAndWaitReplyAsync<Bridge.Message.MsgTmp, Bridge.Message.MsgTmp>(_mqType, "queue2", "Test1", msg);
        }
        
        public async Task Test2Async(System.Collections.Generic.IEnumerable<Bridge.Message.MsgTmp> msgs) {
            await _publisher.PublishAsync(_mqType, "queue2", "Test2", msgs);
        }
        
        public async Task Test3Async() {
            await _publisher.PublishAsync(_mqType, "queue2", "Test3");
        }
        
        public async Task<Bridge.Message.MsgTmp> Test4Async() {
            return await _publisher.PublishAndWaitReplyAsync<Bridge.Message.MsgTmp>(_mqType, "queue2", "Test4");
        }
        
        public async Task<Bridge.Message.MsgTmp> Test5Async(int age) {
            return await _publisher.PublishAndWaitReplyAsync<int, Bridge.Message.MsgTmp>(_mqType, "queue2", "Test5", age);
        }
        
        public async Task<Bridge.Message.MsgTmp> Test6Async(System.Nullable<int> age) {
            return await _publisher.PublishAndWaitReplyAsync<System.Nullable<int>, Bridge.Message.MsgTmp>(_mqType, "queue2", "Test6", age);
        }
        
        public async Task<Bridge.Message.MsgTmp> Test7Async(string name) {
            return await _publisher.PublishAndWaitReplyAsync<string, Bridge.Message.MsgTmp>(_mqType, "queue2", "Test7", name);
        }
        
        public async Task<Bridge.Message.MsgTmp> Test8Async(string name) {
            return await _publisher.PublishAndWaitReplyAsync<string, Bridge.Message.MsgTmp>(_mqType, "queue2", "Test8", name);
        }
    }
}
