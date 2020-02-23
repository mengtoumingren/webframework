using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHost.WebClient.Mvc.Filter
{
    public abstract class AuthorizationFilter : IFilter
    {
        public abstract void OnAuthorization(ActionContext actionContext);
    }
}
