using System.Reflection;

namespace Bridge
{
    public class MQHandlerActionDescriptor
    {
        public required string ActionName { get; set; }
        public required string QueueName { get; set; }
        public required bool IsMulticast { get; set; }
        public MQType MQType { get; set; }
        public required TypeInfo HandlerType { get; set; }
        public required MethodInfo ActionMethodInfo { get; set; }
    }
}
