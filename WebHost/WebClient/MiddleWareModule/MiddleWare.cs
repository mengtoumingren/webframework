using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHost.WebClient.MiddleWareModule
{
    public class MiddleWare<T>
    {
        private List<Func<T, Func<Task>, Task>> MiddleWares = null;
        internal MiddleWare(MiddleWareHandler<T> handler)
        {
            MiddleWares = handler.MiddleWares;
        }

        public void Add(Func<T, Func<Task>, Task> middleWare)
        {
            MiddleWares.Add(middleWare);
        }
        public void Add(IMiddleWare<T> middleWare)
        {
            MiddleWares.Add(async (s, next) =>
            {
                await middleWare.DealWith(s, next);
            });
        }
        
    }
}
