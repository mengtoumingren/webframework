using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class HttpRequest
    {
        public string Url { get; internal set; }
        public string Host { get; internal set; }
        public string Method { get; internal set; }
        public string HttpVersion { get; internal set; }
        public NameValueCollection Headers { get; internal set; }
        public NameValueCollection Cookies { get; internal set; }
        public NameValueCollection Querystring { get; internal set; }
        public NameValueCollection Form { get; internal set; }
        public List<HttpRequestFile> Files { get; internal set; }
        public Stream InputStream { get; internal set; }
    }
}
