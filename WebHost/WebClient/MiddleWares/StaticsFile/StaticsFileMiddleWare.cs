using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.MiddleWareModule;

namespace WebHost.WebClient.MiddleWares.StaticsFile
{
    public class StaticsFileMiddleWare : IMiddleWare<HttpContext>
    {
        private static List<string> StaticsFileExtensionList;
        static StaticsFileMiddleWare()
        {
            //初始化静态后缀列表
            StaticsFileExtensionList = new List<string>()
            {
                ".html"
                ,".jpg"
                ,".jpeg"
                ,".ico"
                ,".png"
                ,".pdf"
                ,".doc"
                ,".xls"
                ,".ppt"
            };
        }
        public async Task DealWith(HttpContext context, Func<Task> next)
        {
            var extension = Path.GetExtension(context.Request.Url);
            //包含静态文件后缀则按静态文件处理
            if (StaticsFileExtensionList.Contains(extension))
            {
                var path =$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{context.Request.Url}";
                if (File.Exists(path))
                {
                    context.Response.WriteFile(path);
                    context.Response.ContentType = "octet-stream";
                    context.Response.Headers["Content-Disposition"] = $"filename=\"{Path.GetFileName(path)}\"";
                }
                else context.Response.State = HttpResponseState.NotFound;
            }
            else await next.Invoke();
        }
    }
}
