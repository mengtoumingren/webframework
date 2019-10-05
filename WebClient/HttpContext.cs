using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class HttpContext
    {
        public HttpRequest Request { get; internal set; }
        public HttpResponse Response { get; internal set; }
        public Session Session { get; internal set; }
        public NameValueCollection RouteData { get; internal set; }

    }
}
