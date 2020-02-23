using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;

namespace WebHost.WebClient.Mvc.ActionResult
{
    public class JsonResult : IActionResult
    {
        private readonly object value;
        private readonly Encoding encoding;

        public JsonResult(object value, Encoding encoding = null)
        {
            this.value = value;
            this.encoding = encoding == null ? Encoding.UTF8 : encoding;
        }

        public void Execute(ActionContext actionContext)
        {
            actionContext.context.Response.ContentType = $"application/json;charset={encoding.BodyName}";
            actionContext.context.Response.Write(JsonConvert.SerializeObject(this.value));
        }
    }
}
