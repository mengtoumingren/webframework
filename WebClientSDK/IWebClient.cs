using System;
using System.Net.Sockets;

namespace WebClientSDK
{
    /// <summary>
    /// 客户端程序约束接口
    /// </summary>
    public interface IWebClient
    {
        void Start(TcpClient client);
    }
}
