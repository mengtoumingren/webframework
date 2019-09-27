using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Reflection;
using WebClientSDK;
using System.Net;
using System.Threading.Tasks;
using WebClient;

namespace WebServer
{
    /// <summary>
    /// 监听服务
    /// </summary>
    public class ListenerService
    {
        private string clientDir;
        private int port;
        private Type clientType;
        private TcpListener listener;
        private IWebClient webClient;
        public ListenerService(string clientDir, int port)
        {
            this.clientDir = clientDir;
            this.port = port;
        }
        /// <summary>
        /// 初始化服务
        /// </summary>
        private void Init()
        {
            //从指定目录下找到处理程序并缓存起来
            if (Directory.Exists(clientDir))
            {
                var files = Directory.GetFiles(clientDir).Where(f => f.EndsWith(".dll"));
                foreach (var file in files)
                {
                    clientType = Assembly.LoadFile(file).GetTypes().Where(t => t.GetInterface(typeof(IWebClient).Name) != null).FirstOrDefault();
                    if (clientType != null) break;
                }
            }
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
            clientType = typeof(Client);//方便测试写死
            if (clientType != null)
            {
                lock("webClient")
                {
                    if (webClient == null)
                        webClient = (IWebClient)Activator.CreateInstance(clientType);
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
