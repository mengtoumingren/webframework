using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc;
using WebClient.Mvc.ActionResult;

namespace AebApp.Controllers
{
    public class TestController : BaseController
    {
        public IActionResult Index()
        {
            return Content("text.index");
        }
        public IActionResult Test()
        {
            return Content("text.text");
        }
    }
}
