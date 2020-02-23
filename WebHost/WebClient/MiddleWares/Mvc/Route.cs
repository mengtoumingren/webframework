using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebHost.WebClient.Mvc
{
    public class Route
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public string DefaultController { get; set; }
        public string DefaultAction { get; set; }

        public bool IsMatch(string url)
        {
            Regex regex = TemplateRegex();
            return regex.IsMatch(url);
        }

        private Regex TemplateRegex()
        {
            var pattern = Template.Replace("{", "(?<").Replace("}", ">[^/,^?]+)");
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex;
        }

        public NameValueCollection GetRouteData(string url)
        {
            Regex regex = TemplateRegex();
            var match =regex.Match(url);
            var routeData = new NameValueCollection();
            var groups =match.Groups;
            for (int i = 1; i < groups.Count; i++)
            {
                dynamic group = groups[i];
                routeData[group.Name] = group.Value;
            }
            return routeData;
        }
    }
}
