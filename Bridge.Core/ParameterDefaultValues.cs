using System.ComponentModel;
using System.Reflection;

namespace Bridge.Core
{
    internal static class ParameterDefaultValues
    {
        public static object?[] GetParameterDefaultValues(MethodBase methodInfo)
        {
            ArgumentNullException.ThrowIfNull(methodInfo);

            var parameters = methodInfo.GetParameters();
            var values = new object?[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                values[i] = GetParameterDefaultValue(parameters[i]);
            }

            return values;
        }

        private static object? GetParameterDefaultValue(ParameterInfo parameterInfo)
        {
            TryGetDeclaredParameterDefaultValue(parameterInfo, out var defaultValue);
            if (defaultValue == null && parameterInfo.ParameterType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(parameterInfo.ParameterType);
            }

            return defaultValue;
        }

        public static bool TryGetDeclaredParameterDefaultValue(ParameterInfo parameterInfo, out object? defaultValue)
        {
            if (ParameterDefaultValue.TryGetDefaultValue(parameterInfo, out defaultValue))
            {
                return true;
            }

            var defaultValueAttribute = parameterInfo.GetCustomAttribute<DefaultValueAttribute>(inherit: false);
            if (defaultValueAttribute != null)
            {
                defaultValue = defaultValueAttribute.Value;
                return true;
            }

            return false;
        }
    }
}
