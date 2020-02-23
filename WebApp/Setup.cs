using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Filters;
using WebHost.WebClient;
using WebHost.WebClient.MiddleWareModule;
using WebHost.WebClient.MiddleWares.Session;
using WebHost.WebClient.MiddleWares.StaticsFile;
using WebHost.WebClient.Mvc;

namespace WebApp
{
    public class Setup : IConfigure
    {
        public void Configure(MiddleWare<HttpContext> app)
        {
            //全局异常处理
            app.Add(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.Response.State = HttpResponseState.InternalServerError;
                    context.Response.ContentType = "text/html";
                    context.Response.Body = new MemoryStream();
                    context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
                    context.Response.Write($"<p>{ex.ToString()}</p>");
                }
            });
            //静态文件处理模型
            app.Add(new StaticsFileMiddleWare());
            //session模块
            app.Add(new SessionMiddleWare());
            //mvc 中间件
            app.Add(new MvcMiddleWare(options=> {

                //路由配置
                options.Routes.Add(new Route
                {
                    Name = "route",
                    Template = "{controller}/{action}/{id}",
                    DefaultController = "home",
                    DefaultAction = "action"
                });
                options.Routes.Add(new Route
                {
                    Name = "route",
                    Template = "{controller}/{action}",
                    DefaultController = "home",
                    DefaultAction = "index"
                });
                //各种过滤器
                options.Filters.Add(new ActionFilter());
                options.Filters.Add(new ActionFilter_1());
                options.Filters.Add(new AuthorizationFilter());
                options.Filters.Add(new AuthorizationFilter_1());
                options.Filters.Add(new ExceptionFilter());
                options.Filters.Add(new ExceptionFilter_1());

            }));

            //app.Add(async (context, next) =>
            //{

            //    context.Response.ContentType = "text/html";
            //    context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
            //    context.Response.Write("<h2>hello world !!</h2>");
            //    context.Response.Write("<h2>你好世界！</h2>");
            //    context.Response.Write($"<h2>sessionId:{context.Request.Cookies["SessionId"]}</h2>");
            //    context.Response.Write("<img src='/bg.jpeg'/>");
            //    //throw new Exception("test excp");
            //});
        }
    }
}
