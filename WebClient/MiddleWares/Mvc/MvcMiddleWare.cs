using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;
using WebClient.Mvc;
using WebClient.Mvc.ActionResult;
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

                var factory =ControllerFactoryProvider.GetControllerFactory();
                var controller =factory.Create(context.Controller);
                controller.context = context.context;
                var param = GetParameters(context.Action.GetParameters(), context);
                var result =context.Action.Invoke(controller, param.Length>0? param:null);
                //有返回值时
                if(result!=null)
                {
                    //是 actionresult类型时的处理
                    if (result is IActionResult)
                    {
                        if(result is IViewResult) ((IViewResult)result).InitView(controller.GetType().Assembly);
                        ((IActionResult)result).Execute(context.context);
                    }
                    //非 actionresult类型时当作对象返回json
                    else new JsonResult(result).Execute(context.context);
                }
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
        /// 从请求中 获取action的参数数据
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static object[] GetParameters(ParameterInfo[] parameters,ActionContext actionContext)
        {
            /*
             * 取值顺序： url-route-form
             */
            var param = new List<Object>();
            //目前暂时取get请求的参数
            foreach (var p in parameters)
            {
                if (!p.ParameterType.IsPrimitive && !p.ParameterType.Name.ToLower().Equals("string"))
                {
                    if(actionContext.context.Request.ContentType.Contains("json"))
                    {
                        try
                        {
                            using (var reader = new StreamReader(actionContext.context.Request.InputStream))
                            {
                                var value = reader.ReadToEnd();
                                var obj = JsonConvert.DeserializeObject(value, p.ParameterType);
                                param.Add(obj);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        var pInstance = Activator.CreateInstance(p.ParameterType);
                        foreach (var property in p.ParameterType.GetProperties())
                        {
                            property.SetValue(pInstance, Convert.ChangeType(GetParameterValue(property.Name.ToLower(),actionContext), null));
                        }
                        param.Add(pInstance);
                    }
                }
                else
                {
                    param.Add(Convert.ChangeType(GetParameterValue(p.Name.ToLower(), actionContext), p.ParameterType));
                }
            }
            return param.ToArray();
        }


        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static object GetParameterValue(string name,ActionContext actionContext)
        {
            object result = null;
            if(actionContext.context.Request.Querystring.AllKeys.Contains(name))
                result = actionContext.context.Request.Querystring[name];
            else if (actionContext.context.RouteData.AllKeys.Contains(name))
                result = actionContext.context.RouteData[name];
            else if (actionContext.context.Request.Form.AllKeys.Contains(name))
                result = actionContext.context.Request.Form[name];
            return result;
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
                foreach (var route in Configuration.Routes)
                {
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
