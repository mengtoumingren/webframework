using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;

namespace WebHost.WebClient.Mvc.ActionResult
{
    public class FileResult : IActionResult
    {
        private readonly string path;
        private readonly string contentType;
        private readonly string contentDisposition;

        public FileResult(string path,string contentType=null,string contentDisposition=null)
        {
            this.path = path;
            this.contentType = contentType;
            this.contentDisposition = contentDisposition;
        }

        public void Execute(ActionContext actionContext)
        {
            actionContext.context.Response.WriteFile(path);
            if(!string.IsNullOrEmpty(contentType))
                actionContext.context.Response.ContentType = contentType;
            if (!string.IsNullOrEmpty(contentType))
                actionContext.context.Response.Headers["Content-Disposition"] = contentDisposition;
        }
    }
}
