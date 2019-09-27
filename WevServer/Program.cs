using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using WebClientSDK;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configs = ClientConfig.Init();
            foreach (var config in configs)
            {
                var service = new ListenerService(config.ClientDir, config.Port);
                service.Start();
            }
            while (true) Console.ReadLine();
        }
    }
}
