using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWares.Mvc.ActionResult
{
    public class JsonResult : IActionResult
    {
        private readonly object value;

        public JsonResult(object value)
        {
            this.value = value;
        }

        public void Execute(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.Write(JsonConvert.SerializeObject(this.value));
        }
    }
}
