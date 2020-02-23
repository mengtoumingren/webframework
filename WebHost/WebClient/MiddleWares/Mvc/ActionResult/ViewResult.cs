using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc.Filter;

namespace WebHost.WebClient.Mvc.ActionResult
{
    public class ViewResult:IActionResult
    {
        private static object loadLock;
        static ViewResult()
        {
            loadLock = new object();
           
        }
        private readonly string viewname;
        private readonly dynamic model;

        public ViewResult()
        {
        }

        public ViewResult(string viewname)
        {
            this.viewname = viewname;
        }
        public ViewResult(dynamic model)
        {
            this.model = model;
        }
        public void Execute(ActionContext actionContext)
        {
            actionContext.context.Response.ContentType = $"text/html ;charset=utf-8";

            var viewdir = $"{Environment.CurrentDirectory}/Views";
            if (!Directory.Exists(viewdir))
            {
                actionContext.context.Response.State = HttpResponseState.NotFound;
                actionContext.context.Response.Write($"不存在的视图路径：{viewdir}/Views");
                return;
            }

            var controller = actionContext.context.RouteData["controller"];

            var controllerDir = Directory.GetDirectories(viewdir).Where(d => d.ToLower().EndsWith(controller.ToLower())).FirstOrDefault();

            if (string.IsNullOrEmpty(controllerDir))
            {
                actionContext.context.Response.State = HttpResponseState.NotFound;
                actionContext.context.Response.Write($"不存在的视图路径：{viewdir}/Views/{controller}");
                return;
            }

            var action = actionContext.context.RouteData["action"];
            var actionfile = Directory.GetFiles(controllerDir).Where(f => f.Replace(Path.GetExtension(f),"").ToLower().EndsWith(action.ToLower())).FirstOrDefault();

            if (string.IsNullOrEmpty(actionfile))
            {
                actionContext.context.Response.State = HttpResponseState.NotFound;
                actionContext.context.Response.Write($"不存在的视图路径：{viewdir}/Views/{controller}/{action}");
                return;
            }

            if(File.Exists(actionfile))
            {
                var template = File.ReadAllText(actionfile, System.Text.Encoding.UTF8);
                var html = Razor.Parse(template, model);
                actionContext.context.Response.State = HttpResponseState.OK;
                actionContext.context.Response.Write(html);
            }
            else
            {
                actionContext.context.Response.State = HttpResponseState.NotFound;
                actionContext.context.Response.Write($"不存在的视图路径：{actionfile}");
            }
        }
    }
}
