using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.Filter;

namespace WebClient.Mvc.Filter
{
    internal class ActionMiddWareContext
    {
        public ActionContext ActionContext { get;set; }
        public Func<Task> Action { get;set; }
    }
}
