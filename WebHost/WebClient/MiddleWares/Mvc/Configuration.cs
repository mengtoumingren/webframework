using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc;
using WebHost.WebClient.Mvc.Filter;

namespace WebHost.WebClient.Mvc
{
    public class Configuration
    {
        public static List<IFilter> Filters;
        public static List<Route> Routes;

        static Configuration()
        {
            Filters = new List<IFilter>();
            Routes = new List<Route>();
        }
    }
}
