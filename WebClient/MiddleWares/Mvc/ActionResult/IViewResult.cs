using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.ActionResult;

namespace WebClient.Mvc.ActionResult
{
    public interface IViewResult:IActionResult
    {
        void InitView(Assembly assembly);
    }
}
