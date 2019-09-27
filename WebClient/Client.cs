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
                            if (context.Request.Files.Count>0)
                            {
                                foreach (var fileInfo in context.Request.Files)
                                {
                                    using (var file = File.Open($"D:/{fileInfo.Filename}", FileMode.Create))
                                    {
                                        byte[] filebuffer = new byte[1024];
                                        while((length = fileInfo.FileStream.Read(filebuffer,0,filebuffer.Length))>0)
                                        {
                                            file.Write(filebuffer, 0, length);
                                            file.Flush();
                                        }

                                    }
                                }
                            }




                            var content = Return();
                            while ((length = content.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                netStream.Write(buffer, 0, length);
                                netStream.Flush();
                            }

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
        private MemoryStream Return()
        {
            MemoryStream stream = new MemoryStream();
            var writer = new StringBuilder();
            writer.Append("HTTP/1.1 200 OK\r\n");
            writer.Append("Date: Wed, 21 Aug 2019 12:45:10 GMT\r\n");
            writer.Append("Content-Disposition: attachment; filename=\"2017%e4%bd%9b%e5%9b%be%e8%b4%ad%e4%b9%b0%e6%95%b0%e6%8d%ae%e5%ba%93%e8%ae%a4%e8%af%81%e7%9b%b8%e5%85%b3%e4%bf%a1%e6%81%af%e5%88%97%e8%a1%a8%ef%bc%88%e6%80%bb%ef%bc%89-%e5%86%85%e7%bd%91%e7%a1%ae%e8%ae%a41.1.xls\"\r\n");
            writer.Append("Content-Length: 54748\r\n");
            writer.Append("\r\n");

            byte[] header = Encoding.UTF8.GetBytes(writer.ToString());
            stream.Write(header, 0, header.Length);


            var file = File.OpenRead(@"C:\Users\mengt\Downloads\2017佛图购买数据库认证相关信息列表（总）-内网确认1.1 (1).xls");
            byte[] buffer = new byte[1024];
            int length = 0;
            while ((length = file.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, length);
            }
            stream.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
