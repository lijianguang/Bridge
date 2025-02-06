using System.Reflection;

namespace Bridge.Abstraction
{
    public interface IProxyGenerator
    {
        void Generate(Assembly assembly, string outPath);
    }
}
