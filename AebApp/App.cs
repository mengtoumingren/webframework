using AebApp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc;

namespace AebApp
{
    public class App : WebClient.WebApplication
    {
        public override void Start()
        {
            //路由配置
            Configuration.Routes.Add(new Route
            {
                Name = "route",
                Template = "{controller}/{action}/{id}",
                DefaultController = "home",
                DefaultAction = "action"
            });
            Configuration.Routes.Add(new Route
            {
                Name = "route",
                Template = "{controller}/{action}",
                DefaultController = "home",
                DefaultAction = "action"
            });
            //各种过滤器
            Configuration.Filters.Add(new ActionFilter());
            Configuration.Filters.Add(new ActionFilter_1());
            Configuration.Filters.Add(new AuthorizationFilter());
            Configuration.Filters.Add(new AuthorizationFilter_1());
            Configuration.Filters.Add(new ExceptionFilter());
            Configuration.Filters.Add(new ExceptionFilter_1());
        }
    }
}
