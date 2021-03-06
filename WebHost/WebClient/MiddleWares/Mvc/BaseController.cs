﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.ActionResult;

namespace WebHost.WebClient.Mvc
{
    public class BaseController
    {
        public HttpContext context { get; internal set; }

        public IActionResult Json(object value)
        {
            return new JsonResult(value);
        }
        public IActionResult Content(string value)
        {
            return new ContentResult(value);
        }
        public IActionResult Json(string path, string contentType = null, string contentDisposition = null)
        {
            return new FileResult(path,contentType,contentDisposition);
        }

        public IActionResult View(string viewname)
        {
            return new ViewResult(viewname);
        }
        public IActionResult View(dynamic model)
        {
            return new ViewResult(model);
        }
        public IActionResult View()
        {
            return new ViewResult();
        }
    }
}
