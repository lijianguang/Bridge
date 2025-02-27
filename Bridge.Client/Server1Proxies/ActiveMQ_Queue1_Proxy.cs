//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server1 {
    
    
    // This proxy's target message queue is ActiveMQ and queue name is queue1
    public class ActiveMQ_Queue1_Proxy : global::Bridge.Abstraction.IHandlerProxy {
        
        private global::Bridge.IPublisher _publisher;
        
        private global::Bridge.MQType _mqType;
        
        public ActiveMQ_Queue1_Proxy(global::Bridge.IPublisher publisher) {
            _publisher = publisher;
            _mqType = global::Bridge.MQType.ActiveMQ;
        }
        
        // This method's action is Test1
        public async Task<System.Collections.Generic.IEnumerable<Bridge.Server1.Models.MsgTmp>> Test1Async(Bridge.Server1.Models.MsgTmp msg) {
            return await _publisher.PublishAndWaitReplyAsync<Bridge.Server1.Models.MsgTmp, System.Collections.Generic.IEnumerable<Bridge.Server1.Models.MsgTmp>>(_mqType, "queue1", "Test1", msg);
        }
        
        // This method's action is Test2
        public async Task Test2Async(System.Collections.Generic.IEnumerable<Bridge.Server1.Models.MsgTmp> msgs, bool needReply = false) {
            await _publisher.PublishAsync(_mqType, "queue1", "Test2", msgs, needReply);
        }
        
        // This method's action is Test3
        public async Task Test3Async(LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage.SalesOrderHeader so, bool needReply = false) {
            await _publisher.PublishAsync(_mqType, "queue1", "Test3", so, needReply);
        }
        
        // This method's action is Test4
        public async Task<Bridge.Server1.Models.MsgTmp> Test4Async() {
            return await _publisher.PublishAndWaitReplyAsync<Bridge.Server1.Models.MsgTmp>(_mqType, "queue1", "Test4");
        }
        
        // This method's action is Test5
        public async Task<Bridge.Server1.Models.MsgTmp> Test5Async(int age) {
            return await _publisher.PublishAndWaitReplyAsync<int, Bridge.Server1.Models.MsgTmp>(_mqType, "queue1", "Test5", age);
        }
        
        // This method's action is Test6
        public async Task<Bridge.Server1.Models.MsgTmp> Test6Async(System.Nullable<int> age) {
            return await _publisher.PublishAndWaitReplyAsync<System.Nullable<int>, Bridge.Server1.Models.MsgTmp>(_mqType, "queue1", "Test6", age);
        }
        
        // This method's action is Test7
        public async Task<Bridge.Server1.Models.MsgTmp> Test7Async(string name) {
            return await _publisher.PublishAndWaitReplyAsync<string, Bridge.Server1.Models.MsgTmp>(_mqType, "queue1", "Test7", name);
        }
        
        // This method's action is Test8
        public async Task<Bridge.Server1.Models.MsgTmp> Test8Async(string name) {
            return await _publisher.PublishAndWaitReplyAsync<string, Bridge.Server1.Models.MsgTmp>(_mqType, "queue1", "Test8", name);
        }
    }
}
