using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Bridge.Core
{
    public class HandlerActionInvoker : IHandlerActionInvoker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MQHandlerActionDescriptor _mqHandlerActionDescriptor;
        private readonly object?[] _models;

        public HandlerActionInvoker(IServiceProvider serviceProvider, MQHandlerActionDescriptor descriptor, object?[] models) 
        {
            _serviceProvider = serviceProvider;
            _mqHandlerActionDescriptor = descriptor;
            _models = models;
        }

        public async Task<object?> InvokeAsync()
        {
            var factory = _serviceProvider.GetRequiredService<IMQHandlerFactoryProvider>().CreateControllerFactory(_mqHandlerActionDescriptor);
            var handler = factory(_serviceProvider); 
            var parameterDefaultValues = ParameterDefaultValues
                .GetParameterDefaultValues(_mqHandlerActionDescriptor.ActionMethodInfo);
            var objectMethodExecutor = ObjectMethodExecutor.Create(
                _mqHandlerActionDescriptor.ActionMethodInfo,
                _mqHandlerActionDescriptor.HandlerType,
                parameterDefaultValues);

            var actionMethodExecutor = ActionMethodExecutor.GetExecutor(objectMethodExecutor);
            var arguments = new Dictionary<string, object?>();

            for(var i = 0; i < _models.Length; i++)
            {
                arguments[_mqHandlerActionDescriptor.ActionMethodInfo.GetParameters()[i].Name ?? ""] = _models[i];
            }
            var orderedArguments = PrepareArguments(arguments, objectMethodExecutor);

            return await actionMethodExecutor.Execute(objectMethodExecutor, handler!, orderedArguments);
        }

        private static object?[]? PrepareArguments(
           IDictionary<string, object?>? actionParameters,
           ObjectMethodExecutor actionMethodExecutor)
        {
            var declaredParameterInfos = actionMethodExecutor.MethodParameters;
            var count = declaredParameterInfos.Length;
            if (count == 0)
            {
                return null;
            }

            Debug.Assert(actionParameters != null, "Expect arguments to be initialized.");

            var arguments = new object?[count];
            for (var index = 0; index < count; index++)
            {
                var parameterInfo = declaredParameterInfos[index];

                if (!actionParameters.TryGetValue(parameterInfo.Name!, out var value) || value is null)
                {
                    value = actionMethodExecutor.GetDefaultValueForParameter(index);
                }

                arguments[index] = value;
            }

            return arguments;
        }
    }
}
