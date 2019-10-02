using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebClientSDK;

namespace WebClient
{
    public class Client : IWebClient
    {
        public void Start(TcpClient client)
        {
            var netStream = client.GetStream();
            MemoryStream stream = new MemoryStream();
            byte[] buffer = new byte[1024];
            int length = 0;
            while (true)
            {
                try
                {


                    length = netStream.Read(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        stream.Write(buffer, 0, length);

                        if (length < buffer.Length || length == 0)
                        {                            
                            stream.Position = 0;

                            byte[] reqData = new byte[stream.Length];
                            stream.Read(reqData, 0, reqData.Length);
                            var msg = Encoding.ASCII.GetString(reqData);
                            
                            Console.WriteLine(msg);
                            var context = HttpContextFactory.CreateHttpContext(msg, reqData);
                            //if (context.Request.Files.Count>0)
                            //{
                            //    foreach (var fileInfo in context.Request.Files)
                            //    {
                            //        using (var file = File.Open($"D:/{fileInfo.Filename}", FileMode.Create))
                            //        {
                            //            byte[] filebuffer = new byte[1024];
                            //            while((length = fileInfo.FileStream.Read(filebuffer,0,filebuffer.Length))>0)
                            //            {
                            //                file.Write(filebuffer, 0, length);
                            //                file.Flush();
                            //            }

                            //        }
                            //    }
                            //}


                            var response = HttpResponseFactory.CreateHttpResponse();
                            var path = @"C:\Users\mengt\Pictures\Screenshots\屏幕截图(1).png";
                            response.WriteFile(path);
                            response.Headers["Content-Disposition"] = $"attachment; filename=\"{Path.GetFileName(path)}\"";
                            HttpResponseFactory.WriteResponse(response, netStream);

                        }
                        buffer = new byte[1024];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
