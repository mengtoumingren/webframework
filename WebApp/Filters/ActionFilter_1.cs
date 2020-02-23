using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;

namespace WebApp.Filters
{
    public class ActionFilter_1 : WebHost.WebClient.Mvc.Filter.ActionFilter
    {
        public override void Executed(ActionContext actionContext)
        {
            Console.WriteLine("ActionFilter_1.Executed");
        }

        public override void OnExecuting(ActionContext actionContext)
        {
            Console.WriteLine("ActionFilter_1.OnExecuting");
        }
    }
}
