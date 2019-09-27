using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    internal class HttpContextFactory
    {
        internal static HttpContext CreateHttpContext(string msg, byte[] reqData)
        {
            var context = new HttpContext();
            context.Request = HttpRequestFactory.CreateHttpRequest(msg,reqData);

            return context;
        }

    }
}
