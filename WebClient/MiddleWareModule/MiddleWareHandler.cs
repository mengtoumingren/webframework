using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.MiddleWareModule
{
    public class MiddleWareHandler<T>
    {
        public List<Func<T, Func<Task>, Task>> MiddleWares = null;
        private bool IsReversed = false;
        private object lockObj =new object();
        public MiddleWareHandler()
        {
            MiddleWares = new List<Func<T, Func<Task>, Task>>();
        }

        internal async Task Execute(T t)
        {
            //列表数据倒序，从最后一个注册的func一层一层往上包
            lock(lockObj)
            {
                //避免每次都反转
                if(!IsReversed)
                {
                    MiddleWares.Reverse();
                    IsReversed = true;
                }
            }
            
            await MiddleWares[MiddleWares.Count - 1].Invoke(t, Execute(t, -1, MiddleWares.Count - 2, null));

        }
        private Func<Task> Execute(T t, int index, int count, Func<Task> func)
        {
            //Console.WriteLine(index+"/"+count);
            if (index < count)
            {
                return Execute(t, ++index, count, async () => await MiddleWares[index].Invoke(t, func));
            }
            return func;
        }
    }
}
