using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost;

namespace WebApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //web启动入口
            new HostService(2345, new Setup()).Start();
        }
    }
}
