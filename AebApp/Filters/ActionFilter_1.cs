using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.Filter;

namespace AebApp.Filters
{
    public class ActionFilter_1 : WebClient.Mvc.Filter.ActionFilter
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
