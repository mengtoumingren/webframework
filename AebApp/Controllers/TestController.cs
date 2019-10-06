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
    public class TestController : BaseController
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return Content("text.index");
        }
        public IActionResult Test()
        {
            return Content("text.text");
        }
        public object Test1()
        {
            return new { code=1,msg= "Test1" };
        }
        public object Test2(string p)
        {
            return new { code = 1, msg =p };
        }
        public object Test3(string p,bool b,int a,string id)
        {
            return new { code = 1, msg = $"{p},{b},{a},{id}" };
        }
        public IActionResult test4(Model model)
        {
            return Json(model);
        }
        public IActionResult test5()
        {
            return new RedirectResult("home/index");
        }

        public class Model
        {
            public string Name { get; set; }
            public int age { get; set; }
        }
    }
}
