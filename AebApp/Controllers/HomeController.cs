﻿using System;
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
        public IActionResult Index()
        {
            return Content("home.index");
        }
    }
}