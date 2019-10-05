using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWares.Mvc.ActionResult;

namespace WebClient.MiddleWares.Mvc.Filter
{
    public class ActionContext
    {
        public HttpContext context { get; set; }
        public Type Controller { get; set; }
        public MethodInfo Action { get; set; }
        public IActionResult Result { get; set; }
    }
}
