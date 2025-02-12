using Bridge.Core;
using Bridge.Sub;

namespace Bridge.Proxy.Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var generator1 = new ProxyGenerator();
            generator1.SetNamespacePrefix("Sub1")
                .Generate(typeof(Queue1Handler).Assembly,
                "C:\\Study\\Bridge\\Bridge.Pub\\Sub1Proxies");

            var generator2 = new ProxyGenerator();
            generator2.SetNamespacePrefix("Sub2")
                .Generate(typeof(Bridge.Sub1.Queue3Handler).Assembly,
                "C:\\Study\\Bridge\\Bridge.Pub\\Sub2Proxies");
        }
    }
}
