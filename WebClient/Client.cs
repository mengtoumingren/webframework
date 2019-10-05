using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClient.MiddleWares.Session;
using WebClient.MiddleWares.StaticsFile;
using WebClient.Mvc;
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

        private void Configure(MiddleWare<HttpContext> app)
        {
            //全局异常处理
            app.Add(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.Response.State = HttpResponseState.InternalServerError;
                    context.Response.ContentType = "text/html";
                    context.Response.Body = new MemoryStream();
                    context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
                    context.Response.Write($"<p>{ex.ToString()}</p>");
                }
            });
            //静态文件处理模型
            app.Add(new StaticsFileMiddleWare());
            //session模块
            app.Add(new SessionMiddleWare());
            //mvc 中间件
            app.Add(new MvcMiddleWare());

            //app.Add(async (context, next) =>
            //{
                
            //    context.Response.ContentType = "text/html";
            //    context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
            //    context.Response.Write("<h2>hello world !!</h2>");
            //    context.Response.Write("<h2>你好世界！</h2>");
            //    context.Response.Write($"<h2>sessionId:{context.Request.Cookies["SessionId"]}</h2>");
            //    context.Response.Write("<img src='/bg.jpeg'/>");
            //    //throw new Exception("test excp");
            //});
        }
    }
}
