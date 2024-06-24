using System.Reflection;

namespace Bridge.Core
{
    public class ProducerDescriptorProvider : IProducerDescriptorProvider
    {
        private static object _lock = new object();
        private IReadOnlyList<ProducerDescriptor>? _producerDescriptors;

        private IReadOnlyList<ProducerDescriptor> ProducerDescriptors
        {
            get
            {
                if (_producerDescriptors == null)
                {
                    lock (_lock)
                    {
                        _producerDescriptors = _producerDescriptors ?? PickupDescriptors<IProducer>((t) => t.GetCustomAttribute<ProducerMappingAttribute>() is ProducerMappingAttribute attribute ? attribute.MQType : MQType.Unknown);
                    }
                }
                return _producerDescriptors;
            }
        }

        private IReadOnlyList<ProducerDescriptor> PickupDescriptors<T>(Func<TypeInfo, MQType> getMQType)
        {
            var descriptors = new List<ProducerDescriptor>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typesFromAssembly = assemblies.Where(x => x.DefinedTypes.Any(t => t.GetInterfaces().Any(i => i == typeof(T)))).SelectMany(x => x.DefinedTypes.Where(t => t.IsClass && t.GetInterfaces().Any(c => c == typeof(T))), (a, t) => t).ToList();
            foreach (var t in typesFromAssembly)
            {
                var mqType = getMQType(t);
                if (mqType != MQType.Unknown)
                {
                    descriptors.Add(new ProducerDescriptor()
                    {
                        MQType = mqType,
                        Type = t,
                    });
                }
            }
            return descriptors;
        }

        public ProducerDescriptor Get(MQType mqType)
        {
            return ProducerDescriptors.FirstOrDefault(h => h.MQType == mqType)
                ?? throw new NotImplementedException($"Can't find the ProducerDescriptor for '{mqType}'");
        }
    }
}
