using System.Reflection;

namespace Bridge.Core
{
    public class ConsumerDescriptorProvider : IConsumerDescriptorProvider
    {
        private static object _lock = new object();
        private IReadOnlyList<ConsumerDescriptor>? _consumerDescriptors;

        private IReadOnlyList<ConsumerDescriptor> ConsumerDescriptors
        {
            get
            {
                if (_consumerDescriptors == null)
                {
                    lock (_lock)
                    {
                        _consumerDescriptors = _consumerDescriptors ?? PickupDescriptors<IConsumer>((t) => t.GetCustomAttribute<ConsumerMappingAttribute>() is ConsumerMappingAttribute attribute ? attribute.MQType : MQType.Unknown);
                    }
                }
                return _consumerDescriptors;
            }
        }

        private IReadOnlyList<ConsumerDescriptor> PickupDescriptors<T>(Func<TypeInfo, MQType> getMQType)
        {
            var descriptors = new List<ConsumerDescriptor>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typesFromAssembly = assemblies.Where(x => x.DefinedTypes.Any(t => t.GetInterfaces().Any(i => i == typeof(T)))).SelectMany(x => x.DefinedTypes.Where(t => t.IsClass && t.GetInterfaces().Any(c => c == typeof(T))), (a, t) => t).ToList();
            foreach (var t in typesFromAssembly)
            {
                var mqType = getMQType(t);
                if (mqType != MQType.Unknown)
                {
                    descriptors.Add(new ConsumerDescriptor()
                    {
                        MQType = mqType,
                        Type = t,
                    });
                }
            }
            return descriptors;
        }

        public ConsumerDescriptor Get(MQType mqType)
        {
            return ConsumerDescriptors.FirstOrDefault(h => h.MQType == mqType)
                ?? throw new NotImplementedException($"Can't find the ConsumerDescriptor for '{mqType}'");
        }
    }
}
