using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bridge.Core
{
    public static class BridgeServiceCollectionExtensions
    {
        public static void AddBridgeServices(this IServiceCollection services)
        {
            services.AddSingleton<ILauncher, Launcher>();

            services.AddSingleton<IMQPipelineBuilder, MQPipelineBuilder>();

            services.AddTransient<IHandlerActionInvoker, HandlerActionInvoker>();
            services.AddTransient<IHandlerMQDelegateFactory, HandlerMQDelegateFactory>();
            services.AddSingleton<IMQHandlerActionDescriptorProvider, MQHandlerActionDescriptorProvider>();
            services.AddTransient<IMQHandlerActivatorProvider, MQHandlerActivatorProvider>();
            services.AddTransient<IMQHandlerFactoryProvider, MQHandlerFactoryProvider>();

            services.AddTransient<IMQEndpointFactory, MQEndpointFactory>();

            services.AddTransient<IMQMiddlewareFactory, MQMiddlewareFactory>();
            services.AddMiddlewares(); 

            services.AddTransient<IMessageConverter, MessageConverter>();

            services.AddTransient<IPublisher, Publisher>();
            services.AddTransient<IProducerFactory, ProducerFactory>();
            services.AddSingleton<IProducerDescriptorProvider, ProducerDescriptorProvider>();
            services.AddTransient<IReplyMessageProcesser, ReplyMessageProcesser>();

            services.AddTransient<ISubscriber, Subscriber>();
            services.AddTransient<IConsumerFactory, ConsumerFactory>();
            services.AddSingleton<IConsumerDescriptorProvider, ConsumerDescriptorProvider>();
        }

        public static void AddMiddlewares(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            services.AddAllImplementedClasses(typeof(IMQMiddleware), (t) => new ServiceDescriptor(typeof(IMQMiddleware), t, lifetime));
        }

        public static void AddAllImplementedClasses(this IServiceCollection services, Type type, Func<TypeInfo, ServiceDescriptor?> generateServiceDescriptor, bool ignoreDuplicateItem = true)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typesFromAssembly = assemblies.Where(x => x.DefinedTypes.Any(t => t.GetInterfaces().Any(c => c == type))).SelectMany(x => x.DefinedTypes.Where(t => !t.IsAbstract && t.IsClass && t.GetInterfaces().Any(c => c == type)), (a, t) => t).ToList();
            foreach (var t in typesFromAssembly)
            {
                if (generateServiceDescriptor(t) is ServiceDescriptor serviceDescriptor)
                {
                    if (ignoreDuplicateItem)
                    {
                        services.TryAdd(serviceDescriptor);
                    }
                    else
                    {
                        services.Add(serviceDescriptor);
                    }
                }
            }
        }
    }
}
