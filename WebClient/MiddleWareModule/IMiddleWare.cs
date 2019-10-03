using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWareModule
{
    public interface IMiddleWare<T>
    {
        Task DealWith(T context, Func<Task> next);
    }
}
