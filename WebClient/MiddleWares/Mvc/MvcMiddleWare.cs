using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClient.MiddleWares.Mvc;
using WebClient.Mvc.Filter;

namespace WebClient.Mvc
{
    internal class MvcMiddleWare : IMiddleWare<HttpContext>
    {
        private static MiddleWareHandler<ActionContext> middleWareHandler;
        private static MiddleWare<ActionContext> middleWare;
        private static Dictionary<string,Type> dicControllerCache;
        private static Dictionary<string,MethodInfo> dicActionCache;
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
                            dicActionCache.Add($"{type.Name.Replace("Controller", "")}.{action.Name}".ToLower(), action);
                            dicControllerCache.Add(type.Name.Replace("Controller", "").ToLower(), type);
                        }
                    }
                }
            }
        }

        private static void InitApplication()
        {
            //从指定目录下找到处理程序并缓存起来
            var rootdir = Assembly.GetExecutingAssembly().Location;
            if (Directory.Exists(rootdir))
            {
                var files = Directory.GetFiles(rootdir).Where(f => f.EndsWith(".dll"));
                foreach (var file in files)
                {
                    //载入所欲继承 basecontroller 的类
                    var appType = Assembly.LoadFile(file).GetTypes().Where(t => t.IsSubclassOf(typeof(WebApplication))).FirstOrDefault();
                    if(appType!=null)
                    {
                        var app =Activator.CreateInstance(appType) as WebApplication;
                        app.Start();
                    }
                }
            }
            
        }

        private static void InitActionFilter()
        {
            actionMiddleWareHandler = new MiddleWareHandler<ActionMiddWareContext>();
            actionMiddleWare = new MiddleWare<ActionMiddWareContext>(actionMiddleWareHandler);
            var filters = Configuration.Filters.Where(f => f.GetType().IsSubclassOf(typeof(ActionFilter))).ToList();
            if (filters.Count > 0)
            {
                filters.Reverse();
                foreach (ActionFilter filter in filters)
                {
                    actionMiddleWare.Add(async (context, next) =>
                    {
                        filter.OnExecuting(context.ActionContext);
                        await context.Action();
                        await next();
                        filter.Executed(context.ActionContext);
                    });
                }
                
            }
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
                    if(filters.Count>0)
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
                            if(expcontext.Result!=null)
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
        }

        public async Task DealWith(HttpContext context, Func<Task> next)
        {
            Configuration.Routes.Add(new Route
            {
                Name = "route1",
                Template = "api/{controller}/{action}",
                DefaultController = "Home",
                DefaultAction = "Index"
            });
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
            if (Configuration.Routes.Count > 0)
            {
                for (int i = Configuration.Routes.Count - 1; i >= 0; i--)
                {
                    var route = Configuration.Routes[i];
                    if (route.IsMatch(context.Request.Url))
                    {
                        isMatch = true;
                        context.RouteData = route.GetRouteData(context.Request.Url);
                        break;
                    }
                }
                //都不匹配的话，取第一个路由的默认值
                if (!isMatch)
                {
                    var route = Configuration.Routes[Configuration.Routes.Count - 1];
                    context.RouteData = new System.Collections.Specialized.NameValueCollection();
                    context.RouteData["controller"] = route.DefaultController;
                    context.RouteData["action"] = route.DefaultAction;
                }
                //执行中间件
                await ExecuteMiddleWare(context);
            }
            else throw new Exception("路由为空！");
        }

        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task ExecuteMiddleWare(HttpContext httpContext)
        {
            if(!!dicActionCache.ContainsKey($"{httpContext.RouteData["controller"]}.{httpContext.RouteData["action"]}")) throw new Exception("路由为空！");

            var actionContext = new ActionContext();
            actionContext.context = httpContext;
            actionContext.Controller = dicControllerCache[httpContext.RouteData["controller"]];
            actionContext.Action = dicActionCache[httpContext.RouteData["action"]];
            await middleWareHandler.Execute(actionContext);
        }
    }
}
