using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.MiddleWareModule;

namespace WebHost.WebClient
{
    public interface IConfigure
    {
        void Configure(MiddleWare<HttpContext> app);
    }
}
