using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClientSDK;

namespace WebClient
{
    public class Client : IWebClient
    {
        private MiddleWareHandler<HttpContext> middleWareHandler;
        private MiddleWare<HttpContext> middleWare;
        public Client()
        {
            //初始化中间件
            middleWareHandler = new MiddleWareHandler<HttpContext>();
            middleWare = new MiddleWare<HttpContext>(middleWareHandler);
            Configure(middleWare);
        }

        public void  Start(TcpClient client)
        {
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
        }

        private void Configure(MiddleWare<HttpContext> app)
        {
            app.Add(async (context, next) =>
            {
                var response = context.Response;
                var path = @"C:\Users\mengt\Pictures\Screenshots\屏幕截图(1).png";
                response.WriteFile(path);
                response.ContentType = "octet-stream";
                response.Headers["Content-Disposition"] = $"filename=\"{Path.GetFileName(path)}\"";
            });
        }
    }
}
