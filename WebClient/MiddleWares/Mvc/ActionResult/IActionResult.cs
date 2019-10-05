using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWares.Mvc.ActionResult
{
    public interface IActionResult
    {
        void Execute(HttpContext httpContext);
    }
}
