using WebApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc;
using WebHost.WebClient.Mvc.ActionResult;

namespace WebApp.Controllers
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
