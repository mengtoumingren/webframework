using AebApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc;
using WebClient.Mvc.ActionResult;

namespace AebApp.Controllers
{
    public class HomeController: BaseController
    {
        [AllowAnonymous]
        public void Index()
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
            context.Response.Write("<h2>hello world !!</h2>");
            context.Response.Write("<h2>你好世界！</h2>");
            context.Response.Write($"<h2>sessionId:{context.Request.Cookies["SessionId"]}</h2>");
            context.Response.Write("<img src='/bg.jpeg'/>");
        }
    }
}
