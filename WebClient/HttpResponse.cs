using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class HttpResponse
    {
        public string HttpVersion { get; internal set; }
        public NameValueCollection Headers { get; internal set; }
        public  Stream Body { get;  internal set; }
        public NameValueCollection Cookies { get; internal set; }
        public HttpResponseState State { get; set; }

        public string ContentType { get; set; }



        public void Write(byte[] buffer, int offset, int count)
        {
            Body.Write(buffer, offset, count);
        }
        public void Write(string value,Encoding encoding)
        {
            byte[] header = encoding.GetBytes(value);
            Body.Write(header, 0, header.Length);
        }
        public void Write(string value)
        {
            Write(value, Encoding.UTF8);
        }
        public void WriteFile(string path)
        {
            using (var file = File.OpenRead(path))
            {
                byte[] buffer = new byte[1024];
                int length = 0;
                while ((length = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Body.Write(buffer, 0, length);
                }
            }
            Body.Flush();
        }


    }
}
