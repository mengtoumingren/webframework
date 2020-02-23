using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;

namespace WebApp.Filters
{
    public class ActionFilter : WebHost.WebClient.Mvc.Filter.ActionFilter
    {
        public override void Executed(ActionContext actionContext)
        {
            Console.WriteLine("ActionFilter.Executed");
        }

        public override void OnExecuting(ActionContext actionContext)
        {
            Console.WriteLine("ActionFilter.OnExecuting");
        }
    }
}
