using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClient.MiddleWares.Mvc;

namespace WebClient.MiddleWares.Session
{
    internal class MvcMiddleWare : IMiddleWare<HttpContext>
    {
        private static MiddleWareHandler<HttpContext> middleWareHandler;
        private static MiddleWare<HttpContext> middleWare;
        private static Dictionary<string,Type> dicControllerCache;
        private static Dictionary<string,MethodInfo> dicActionCache;
        static MvcMiddleWare()
        {
            //初始化中间件
            middleWareHandler = new MiddleWareHandler<HttpContext>();
            middleWare = new MiddleWare<HttpContext>(middleWareHandler);
            Configure(middleWare);
            //载入缓存
            LoadCache();
        }

        /// <summary>
        /// 将controller action载入缓存
        /// </summary>
        private static void LoadCache()
        {
            dicControllerCache = new Dictionary<string, Type>();
            dicActionCache =new Dictionary<string, MethodInfo>();

            //从指定目录下找到处理程序并缓存起来
            var rootdir = Assembly.GetExecutingAssembly().Location;
            if (Directory.Exists(rootdir))
            {
                var files = Directory.GetFiles(rootdir).Where(f => f.EndsWith(".dll"));
                foreach (var file in files)
                {
                    //载入所欲继承 basecontroller 的类
                    var controllerTypes = Assembly.LoadFile(file).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseController))&&t.Name.EndsWith("Controller"));

                    foreach (var type in controllerTypes)
                    {
                        var methods =type.GetMethods(BindingFlags.Public);
                        var actions = new List<string>();
                        foreach (var action in methods)
                        {
                            actions.Add(action.Name);
                            dicActionCache.Add($"{type.Name.Replace("Controller", "")}.{action.Name}", action);
                            dicControllerCache.Add(type.Name.Replace("Controller", ""), type);
                        }
                    }
                }
            }
        }

        private static void Configure(MiddleWare<HttpContext> app)
        {

            //授权认证
            app.Add(async (context, next) =>
            {
                await next();
            });

            //异常过滤器
            app.Add(async (context, next) =>
            {
                await next();
            });

            //执行前
            app.Add(async (context, next) =>
            {
                await next();
            });

            //执行后
            app.Add(async (context, next) =>
            {
                await next();
            });
        }

        public async Task DealWith(HttpContext context, Func<Task> next)
        {
            var sessionId =context.Request.Cookies["SessionId"];
            if(string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                context.Response.Cookies["SessionId"] = sessionId;
            }
            context.Session = new WebClient.Session(sessionId);
        }
    }
}
