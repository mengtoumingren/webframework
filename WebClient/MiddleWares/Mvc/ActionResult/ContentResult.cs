using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWares.Mvc.ActionResult
{
    public class ContentResult : IActionResult
    {
        private string Content;
        public ContentResult(string Content)
        {
            this.Content = Content;
        }
        
        public void Execute(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "text/html";
            httpContext.Response.Write(Content);
        }
    }
}
