using Bridge.Core;
using Bridge.Server1.Handlers;

namespace Bridge.Proxy.Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var generator1 = new ProxyGenerator();
            generator1.SetNamespacePrefix("Server1")
                .Generate(typeof(Queue1Handler).Assembly,
                "C:\\Study\\Bridge\\Bridge.Client\\Server1Proxies");

            var generator2 = new ProxyGenerator();
            generator2.SetNamespacePrefix("Server2")
                .Generate(typeof(Server2.Handlers.Queue3MulticastHandler).Assembly,
                "C:\\Study\\Bridge\\Bridge.Client\\Server2Proxies");
        }
    }
}
