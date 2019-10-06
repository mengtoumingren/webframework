using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.ActionResult;

namespace WebClient.Mvc.ActionResult
{
    public class ViewResult : IViewResult
    {
        private static Dictionary<string, Type> dicViews;
        private static object loadLock;
        static ViewResult()
        {
            loadLock = new object();
           
        }
        private static void LoadViewCache(Assembly assembly)
        {
            //缓存视图
            dicViews = new Dictionary<string, Type>();
            foreach (var type in assembly.GetTypes().Where(t => t.FullName.Contains(".Views.")))
            {
                dicViews.Add(type.FullName, type);
            }
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
        public void Execute(HttpContext httpContext)
        {
            httpContext.Response.ContentType = $"text/html ;charset=utf-8";


            var viewTypeName = $".Views.{httpContext.RouteData["controller"]}.{(viewname==null? httpContext.RouteData["action"]:viewname)}".ToLower();
            var key =dicViews.Keys.Where(k => k.ToLower().EndsWith(viewTypeName)).FirstOrDefault();
            if(dicViews.ContainsKey(key))
            {
                dynamic page = Activator.CreateInstance(dicViews[key], model == null ? null : new object[] { model });
                var pageContent = page.TransformText();
                httpContext.Response.Write(pageContent);
            }
            else
            {
                httpContext.Response.State = HttpResponseState.NotFound;
                httpContext.Response.Write($"不存在的视图路径：{viewTypeName}");
            }
        }

        public void InitView(Assembly assembly)
        {
            lock (loadLock)
            {
                if (dicViews == null)
                {
                    LoadViewCache(assembly);
                }
            }
        }
    }
}
