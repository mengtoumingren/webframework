using AebApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc;
using WebClient.Mvc.ActionResult;

namespace AebApp.Controllers
{
    public class HomeController: BaseController
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(new { Session = context.Request.Cookies["SessionId"] });
        }
    }
}
