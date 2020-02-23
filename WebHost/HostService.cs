using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Threading.Tasks;
using WebHost.WebClient;

namespace WebHost
{
    /// <summary>
    /// 监听服务
    /// </summary>
    public class HostService
    {
        private int port;
        private Type clientType;
        private TcpListener listener;
        private Client webClient;
        private IConfigure configure;
        public HostService( int port, IConfigure configure)
        {
            this.port = port;
            this.configure = configure;
        }
        /// <summary>
        /// 启用服务
        /// </summary>
        public void Start()
        {
            //Init();
            //ip地址，0.0.0.0 表示开启本地地址
            string serverIP = "0.0.0.0";
            listener = new TcpListener(IPAddress.Parse(serverIP), port);
            listener.Start();
            Console.WriteLine($"开启服务：{serverIP}:{port}");
            clientType = typeof(Client);//方便测试写死
            if (clientType != null)
            {
                lock("webClient")
                {
                    if (webClient == null)
                        webClient = new Client(configure);
                }

                while (true)
                {
                    //AcceptTcpClient会进行阻塞，直到有请求进来
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    //开启异步处理
                    Task.Factory.StartNew(new Action<object>(obj =>
                    {
                        TcpClient cli = obj as TcpClient;
                        webClient.Start(cli);
                    }), tcpClient);
                }
            }

        }
    }
}
