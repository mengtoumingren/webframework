using AebApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc.ActionResult;
using WebClient.Mvc.Filter;

namespace AebApp.Filters
{
    public class AuthorizationFilter : WebClient.Mvc.Filter.AuthorizationFilter
    {
        public override void OnAuthorization(ActionContext actionContext)
        {
            if (actionContext.Action.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Count() > 0) return;
            if (actionContext.Controller.Name.ToLower().Contains("test")) return;

            actionContext.Result = new ContentResult("AuthorizationFilter 授权不通过");
        }
    }
}
