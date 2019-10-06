using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClient.Mvc;
using WebClient.Mvc.Filter;

namespace WebClient.Mvc
{
    internal class MvcMiddleWare : IMiddleWare<HttpContext>
    {
        private static MiddleWareHandler<ActionContext> middleWareHandler;
        private static MiddleWare<ActionContext> middleWare;
        private static Dictionary<string, Type> dicControllerCache;
        private static Dictionary<string, MethodInfo> dicActionCache;
        //action filter 中间件
        private static MiddleWareHandler<ActionMiddWareContext> actionMiddleWareHandler;
        private static MiddleWare<ActionMiddWareContext> actionMiddleWare;
        static MvcMiddleWare()
        {
            //初始化中间件
            middleWareHandler = new MiddleWareHandler<ActionContext>();
            middleWare = new MiddleWare<ActionContext>(middleWareHandler);
            Configure(middleWare);
            //载入缓存
            LoadCache();
            //初始化应用
            InitApplication();
            //初始化动作过滤器
            InitActionFilter();

            Console.WriteLine("启动成功。。。");
        }

        /// <summary>
        /// 将controller action载入缓存
        /// </summary>
        private static void LoadCache()
        {
            dicControllerCache = new Dictionary<string, Type>();
            dicActionCache = new Dictionary<string, MethodInfo>();

            //从指定目录下找到处理程序并缓存起来
            var rootdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (Directory.Exists(rootdir))
            {
                var files = Directory.GetFiles(rootdir).Where(f => f.EndsWith(".dll"));
                foreach (var file in files)
                {
                    //载入所欲继承 basecontroller 的类
                    var controllerTypes = Assembly.LoadFile(file).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseController)) && t.Name.EndsWith("Controller"));

                    foreach (var type in controllerTypes)
                    {
                        var methods = type.GetMethods();
                        foreach (var action in methods)
                        {
                            var actionName = $"{type.Name.Replace("Controller", "")}.{action.Name}".ToLower();
                            if (!dicActionCache.ContainsKey(actionName))
                                dicActionCache.Add(actionName, action);
                            var controllerName = type.Name.Replace("Controller", "").ToLower();
                            if (!dicControllerCache.ContainsKey(controllerName))
                                dicControllerCache.Add(controllerName, type);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 初始化web应用
        /// </summary>
        private static void InitApplication()
        {
            //从指定目录下找到处理程序并缓存起来
            var rootdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (Directory.Exists(rootdir))
            {
                var files = Directory.GetFiles(rootdir).Where(f => f.EndsWith(".dll"));
                foreach (var file in files)
                {
                    //载入所欲继承 basecontroller 的类
                    var appType = Assembly.LoadFile(file).GetTypes().Where(t => t.IsSubclassOf(typeof(WebApplication))).FirstOrDefault();
                    if (appType != null)
                    {
                        var app = Activator.CreateInstance(appType) as WebApplication;
                        app.Start();
                    }
                }
            }

        }
        /// <summary>
        /// 初始化动作过滤器
        /// </summary>
        private static void InitActionFilter()
        {
            actionMiddleWareHandler = new MiddleWareHandler<ActionMiddWareContext>();
            actionMiddleWare = new MiddleWare<ActionMiddWareContext>(actionMiddleWareHandler);
            var filters = Configuration.Filters.Where(f => f.GetType().IsSubclassOf(typeof(ActionFilter))).ToList();
            if (filters.Count > 0)
            {
                foreach (ActionFilter filter in filters)
                {
                    actionMiddleWare.Add(async (context, next) =>
                    {
                        filter.OnExecuting(context.ActionContext);
                        await next();
                        filter.Executed(context.ActionContext);
                    });
                }
            }
            //实际要调用继续往下调用的中间件
            actionMiddleWare.Add(async (context, next) =>
            {
                await context.Action();
            });
        }

        private static void Configure(MiddleWare<ActionContext> app)
        {
            //异常过滤器
            app.Add(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    var filters = Configuration.Filters.Where(f => f.GetType().IsSubclassOf(typeof(ExceptionFilter))).ToList();
                    if (filters.Count > 0)
                    {
                        var expcontext = new ExceptionContext();
                        expcontext.context = context.context;
                        expcontext.Controller = context.Controller;
                        expcontext.Action = context.Action;
                        expcontext.Result = context.Result;
                        expcontext.exception = ex;
                        foreach (ExceptionFilter filter in filters)
                        {
                            filter.Resole(expcontext);
                            if (expcontext.Result != null)
                            {
                                expcontext.Result.Execute(expcontext.context);
                                break;
                            }
                        }
                        if (expcontext.Result == null) throw;
                    }
                    else throw;
                }
            });

            //授权认证
            app.Add(async (context, next) =>
            {
                var isAuthorized = true;
                var filters = Configuration.Filters.Where(f => f.GetType().IsSubclassOf(typeof(AuthorizationFilter))).ToList();
                if (filters.Count > 0)
                {
                    foreach (AuthorizationFilter filter in filters)
                    {
                        filter.OnAuthorization(context);
                        if (context.Result != null)
                        {
                            isAuthorized = false;
                            context.Result.Execute(context.context);
                            break;
                        }
                    }
                    if (isAuthorized) await next();
                }
                else await next();
            });

            //行为过滤器
            app.Add(async (context, next) =>
            {
                await actionMiddleWareHandler.Execute(new ActionMiddWareContext { Action = next, ActionContext = context });
            });

            //核心逻辑
            app.Add(async (context, next) =>
            {
                Console.WriteLine("core code");

                throw new Exception("构造的异常");

                context.context.Response.ContentType = "text/html";
                context.context.Response.Write("<head><meta http-equiv=\"content-type\" content=\"text/html; charset =utf-8\" /></head>");
                context.context.Response.Write("<h2>hello world !!</h2>");
                context.context.Response.Write("<h2>你好世界！</h2>");
                context.context.Response.Write($"<h2>sessionId:{context.context.Request.Cookies["SessionId"]}</h2>");
                context.context.Response.Write("<img src='/bg.jpeg'/>");
            });

        }
        /// <summary>
        /// 中间件处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task DealWith(HttpContext context, Func<Task> next)
        {
            await MatchRoute(context);
        }

        /// <summary>
        /// 匹配路由
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task MatchRoute(HttpContext context)
        {
            var isMatch = false;
            Route matchRoute = null;
            if (Configuration.Routes.Count > 0)
            {
                for (int i = Configuration.Routes.Count - 1; i >= 0; i--)
                {
                    var route = Configuration.Routes[i];
                    if (route.IsMatch(context.Request.Url))
                    {
                        matchRoute = route;
                        isMatch = true;
                        context.RouteData = route.GetRouteData(context.Request.Url);
                        break;
                    }
                }
                //都不匹配的话，
                if (!isMatch) context.Response.State = HttpResponseState.NotFound;
                else
                {
                    //如果匹配但是控制器和动作匹配不上则采用默认action
                    if (context.RouteData["controller"] == null) context.RouteData["controller"] = matchRoute.DefaultController.ToLower();
                    if (context.RouteData["action"] == null) context.RouteData["action"] = matchRoute.DefaultController.ToLower();
                }
                //执行中间件
                await ExecuteMiddleWare(context);
            }
            else context.Response.State = HttpResponseState.NotFound;
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task ExecuteMiddleWare(HttpContext httpContext)
        {
            if (!dicActionCache.ContainsKey($"{httpContext.RouteData["controller"]}.{httpContext.RouteData["action"]}"))
                httpContext.Response.State = HttpResponseState.NotFound;
            else
            {
                var actionContext = new ActionContext();
                actionContext.context = httpContext;
                actionContext.Controller = dicControllerCache[httpContext.RouteData["controller"].ToLower()];
                actionContext.Action = dicActionCache[$"{httpContext.RouteData["controller"]}.{httpContext.RouteData["action"]}".ToLower()];
                await middleWareHandler.Execute(actionContext);
            }
        }
    }
}
