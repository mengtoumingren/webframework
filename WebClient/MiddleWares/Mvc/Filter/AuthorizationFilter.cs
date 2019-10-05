using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWares.Mvc.Filter
{
    public abstract class AuthorizationFilter
    {
        public abstract void OnAuthorization(ActionContext actionContext);
    }
}
