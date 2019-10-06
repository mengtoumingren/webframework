using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Mvc.ActionResult
{
    public class ContentResult : IActionResult
    {
        private string Content;
        private readonly Encoding encoding;

        public ContentResult(string Content,Encoding encoding=null)
        {
            this.Content = Content;
            this.encoding = encoding==null?Encoding.UTF8:encoding;
        }
        
        public void Execute(HttpContext httpContext)
        {
            httpContext.Response.ContentType = $"text/html ;charset={encoding.BodyName}";
            httpContext.Response.Write(Content);
        }
    }
}
