using System.Reflection;

namespace Bridge.Core
{
    public class MQHandlerActionDescriptorProvider : IMQHandlerActionDescriptorProvider
    {
        private static object _lock = new object();
        private IReadOnlyList<MQHandlerActionDescriptor>? _mqActionDescriptors;

        protected IReadOnlyList<MQHandlerActionDescriptor> MQActionDescriptors
        {
            get
            {
                if (_mqActionDescriptors == null)
                {
                    lock (_lock)
                    {
                        _mqActionDescriptors = _mqActionDescriptors ?? PickupDescriptors<IMQHandler>((t) =>
                        {
                            var attribute = t.GetCustomAttribute<MQHandlerAttribute>();
                            return attribute != null ? (attribute.MQType, attribute.QueueName, attribute.IsMulticast) : (MQType.Unknown, string.Empty, false);
                        });
                    }
                }
                return _mqActionDescriptors;
            }
        }

        private IReadOnlyList<MQHandlerActionDescriptor> PickupDescriptors<T>(Func<TypeInfo, (MQType, string, bool)> getHandlerInfo)
        {
            var descriptors = new List<MQHandlerActionDescriptor>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typesFromAssembly = assemblies.Where(x => x.DefinedTypes.Any(t => t.GetInterfaces().Any(i => i == typeof(T)))).SelectMany(x => x.DefinedTypes.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(c => c == typeof(T))), (a, t) => t).ToList();

            foreach (var t in typesFromAssembly)
            {
                var handlerInfo = getHandlerInfo(t);

                var methods = t.GetMethods();

                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<MQActionAttribute>();

                    if (attribute != null && handlerInfo.Item1 != MQType.Unknown && !string.IsNullOrEmpty(handlerInfo.Item2))
                    {
                        descriptors.Add(new MQHandlerActionDescriptor()
                        {
                            QueueName = handlerInfo.Item2,
                            MQType = handlerInfo.Item1,
                            IsMulticast = handlerInfo.Item3,
                            HandlerType = t,
                            ActionName = attribute.Action,
                            ActionMethodInfo = method,
                        });
                    }
                }
                
            }
            return descriptors;
        }

        public MQHandlerActionDescriptor Get(MQType mqType, string queueName, string actionName)
        {
            return Get(mqType, queueName).FirstOrDefault(x => x.ActionName == actionName)
                ?? throw new NotImplementedException($"Can't resolve the MQActionDescriptor for MQType '{mqType}', queueName '{queueName}', actionName '{actionName}'");
        }

        public IEnumerable<MQHandlerActionDescriptor> Get(MQType mqType, string queueName)
        {
            return MQActionDescriptors.Where(h => h.MQType == mqType && h.QueueName == queueName);
        }

        public IEnumerable<MQHandlerActionDescriptor> Get(MQType mqType)
        {
            return MQActionDescriptors.Where(h => h.MQType == mqType);
        }

        public IEnumerable<MQHandlerActionDescriptor> Get()
        {
            return MQActionDescriptors;
        }
    }
}
