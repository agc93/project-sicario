using System;
using System.Threading.Tasks;

namespace SicarioPatch.Loader
{
    class Program
    {
        static Task Main(string[] args) {
            return Startup.GetApp().RunAsync(args);
        }
    }
}