using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;
using WebHost.WebClient.Mvc.ActionResult;

namespace WebApp.Filters
{
    public class ExceptionFilter_1 : WebHost.WebClient.Mvc.Filter.ExceptionFilter
    {
        public override void Resole(ExceptionContext exceptionContext)
        {
            Console.WriteLine(this.GetType().FullName);
            exceptionContext.Result = new JsonResult(new { msg = "error_1", code = 0 ,ex= exceptionContext .exception.ToString()});
        }
    }
}
