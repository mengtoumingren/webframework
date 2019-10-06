using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.ActionResult;
using WebClient.Mvc.Filter;

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
        public void Execute(ActionContext actionContext)
        {
            actionContext.context.Response.ContentType = $"text/html ;charset=utf-8";


            var viewTypeName = $".Views.{actionContext.context.RouteData["controller"]}.{(viewname==null? actionContext.context.RouteData["action"]:viewname)}".ToLower();
            var key =dicViews.Keys.Where(k => k.ToLower().EndsWith(viewTypeName)).FirstOrDefault();
            if(dicViews.ContainsKey(key))
            {
                dynamic page = Activator.CreateInstance(dicViews[key], model == null ? null : new object[] { model });
                var pageContent = page.TransformText();
                actionContext.context.Response.Write(pageContent);
            }
            else
            {
                actionContext.context.Response.State = HttpResponseState.NotFound;
                actionContext.context.Response.Write($"不存在的视图路径：{viewTypeName}");
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
