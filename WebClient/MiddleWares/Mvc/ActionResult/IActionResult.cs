using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.Filter;

namespace WebClient.Mvc.ActionResult
{
    public interface IActionResult
    {
        void Execute(ActionContext httpContext);
    }
}
