using System;
using Microsoft.Owin.Hosting;

namespace SelfHostSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = args.Length == 0 ? "http://localhost:8081/" : args[0];

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started");
                Console.ReadKey();
                Console.WriteLine("Stopping");
            }
        }
    }
}
