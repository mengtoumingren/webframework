﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Extends;

namespace WebHost.WebClient
{
    public class HttpResponseFactory
    {
        /// <summary>
        /// 初始化 response
        /// </summary>
        /// <returns></returns>
        public static HttpResponse CreateHttpResponse()
        {
            var rsp = new HttpResponse();
            rsp.Body = new MemoryStream();
            rsp.Body.Position = 0;
            rsp.HttpVersion = "HTTP/1.1";
            rsp.Headers = new System.Collections.Specialized.NameValueCollection();
            rsp.State = HttpResponseState.OK;
            rsp.Cookies = new System.Collections.Specialized.NameValueCollection();
            return rsp;
        }

        /// <summary>
        /// 将response写入返回
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="networkStream"></param>
        public static void WriteResponse(HttpResponse httpResponse, NetworkStream networkStream)
        {
            var writer = new StringBuilder();
            writer.AppendFormat("{0} {1} {2}\r\n", httpResponse.HttpVersion, (int)httpResponse.State, httpResponse.State.Remark());
            writer.AppendFormat("Date: {0}\r\n", DateTime.Now.ToUniversalTime());

            foreach (var key in httpResponse.Headers.AllKeys)
            {
                if (key.ToLower().Equals("content-type")) continue;
                if (key.ToLower().Equals("content-length")) continue;
                if (key.ToLower().Equals("set-cookie")) continue;
                writer.AppendFormat("{0}:{1}\r\n", key, httpResponse.Headers[key]);
            }
            //cookies
            if(httpResponse.Cookies.Count>0)
            {
                var builder = new StringBuilder();
                foreach (var key in httpResponse.Cookies.AllKeys)
                    builder.AppendFormat("{0}={1};", key, httpResponse.Cookies[key]);
                builder.Append("path=/;");
                writer.AppendFormat("Set-Cookie:{0}\r\n", builder.ToString());
            }

            writer.AppendFormat("Content-Type:{0}\r\n", httpResponse.ContentType);
            writer.AppendFormat("Content-Length:{0}\r\n", httpResponse.Body.Length);
            writer.Append("\r\n");

            byte[] header = Encoding.UTF8.GetBytes(writer.ToString());
            networkStream.Write(header, 0, header.Length);

            byte[] buffer = new byte[1024];
            var length = 0;
            httpResponse.Body.Position = 0;
            while ((length = httpResponse.Body.Read(buffer, 0, buffer.Length)) > 0)
            {
                networkStream.Write(buffer, 0, length);
                networkStream.Flush();
            }
        }
    }
}
