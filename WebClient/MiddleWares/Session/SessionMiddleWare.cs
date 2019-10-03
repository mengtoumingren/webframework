﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebClient.MiddleWareModule;

namespace WebClient.MiddleWares.Session
{
    internal class SessionMiddleWare : IMiddleWare<HttpContext>
    {
        public async Task DealWith(HttpContext context, Func<Task> next)
        {
            var sessionId =context.Request.Cookies["SessionId"];
            if(string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                context.Response.Cookies["SessionId"] = sessionId;
            }
            context.Session = new WebClient.Session(sessionId);

            await next();
        }
    }
}
