using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;
using WebHost.WebClient.Mvc.ActionResult;

namespace WebApp.Filters
{
    public class ExceptionFilter : WebHost.WebClient.Mvc.Filter.ExceptionFilter
    {
        public override void Resole(ExceptionContext exceptionContext)
        {
            Console.WriteLine(this.GetType().FullName);
        }
    }
}
