using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.Filter;
using WebClient.Mvc.ActionResult;

namespace AebApp.Filters
{
    public class ExceptionFilter : WebClient.Mvc.Filter.ExceptionFilter
    {
        public override void Resole(ExceptionContext exceptionContext)
        {
            Console.WriteLine(this.GetType().FullName);
        }
    }
}
