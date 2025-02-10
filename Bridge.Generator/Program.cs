using Bridge.Core;
using Bridge.Sub;

namespace Bridge.Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var generator = new ProxyGenerator();

            generator.Generate(typeof(Queue1Handler).Assembly, 
                "C:\\Study\\Bridge\\Bridge.Proxy\\Proxies");
        }
    }
}
