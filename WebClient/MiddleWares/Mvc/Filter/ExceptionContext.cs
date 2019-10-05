using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Mvc.Filter
{
    public class ExceptionContext: ActionContext
    {
        public Exception exception { get; set; }
    }
}
