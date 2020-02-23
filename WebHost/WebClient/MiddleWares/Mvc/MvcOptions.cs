using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc;
using WebHost.WebClient.Mvc.Filter;

namespace WebHost.WebClient.MiddleWares.Mvc
{
    public class MvcOptions
    {
        public List<IFilter> Filters;
        public List<Route> Routes;
    }
}
