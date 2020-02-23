using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.ActionResult;

namespace WebHost.WebClient.Mvc.Filter
{
    public class ActionContext
    {
        public HttpContext context { get; set; }
        public Type Controller { get; set; }
        public MethodInfo Action { get; set; }
        public IActionResult Result { get; set; }
        public Route Route { get; set; }
    }
}
