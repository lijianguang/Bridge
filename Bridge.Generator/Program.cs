using Bridge.Core;
using Bridge.Sub.Handlers;

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
                .Generate(typeof(Sub2.Handlers.Queue3MulticastHandler).Assembly,
                "C:\\Study\\Bridge\\Bridge.Pub\\Sub2Proxies");
        }
    }
}
