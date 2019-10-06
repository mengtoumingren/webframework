using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Mvc.Filter
{
    public abstract class ExceptionFilter : IFilter
    {
        public abstract void Resole(ExceptionContext exceptionContext);
    }
}
