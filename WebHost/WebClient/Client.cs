using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.MiddleWareModule;
using WebHost.WebClient.MiddleWares.Session;
using WebHost.WebClient.MiddleWares.StaticsFile;
using WebHost.WebClient.Mvc;

namespace WebHost.WebClient
{
    public class Client 
    {
        private MiddleWareHandler<HttpContext> middleWareHandler;
        private MiddleWare<HttpContext> middleWare;
        public Client(IConfigure configure)
        {
            //初始化中间件
            middleWareHandler = new MiddleWareHandler<HttpContext>();
            middleWare = new MiddleWare<HttpContext>(middleWareHandler);
            configure.Configure(middleWare);
        }

        public void  Start(TcpClient client)
        {
            #region 
            var netStream = client.GetStream();
            MemoryStream stream = new MemoryStream();
            byte[] buffer = new byte[1024];
            byte[] emptybuffer = new byte[1024];
            int length = 0;
            while (true)
            {
                try
                {
                    //从网络流获取数据
                    length = netStream.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        stream.Write(buffer, 0, length);
                        //当次请求的数据获取完成
                        if (length < buffer.Length || length == 0)
                        {
                            stream.Position = 0;
                            byte[] reqData = new byte[stream.Length];
                            stream.Read(reqData, 0, reqData.Length);
                            var msg = Encoding.ASCII.GetString(reqData);
                            //Console.WriteLine(msg);
                            //初始化请求上下文（贯穿请求处理的对象）
                            var context = HttpContextFactory.CreateHttpContext(msg, reqData);
                            Console.WriteLine(context.Request.Url);
                            //执行中间件
                            Task.WaitAll(middleWareHandler.Execute(context));
                            //处理相应数据
                            HttpResponseFactory.WriteResponse(context.Response, netStream);

                            //重置数据
                            buffer = (byte[])emptybuffer.Clone();
                            stream = new MemoryStream();
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            #endregion
        }
    }
}
