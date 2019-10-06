using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Mvc.Filter
{
    public abstract class ActionFilter:IFilter
    {
        /// <summary>
        /// 执行中（前）
        /// </summary>
        /// <param name="actionContext"></param>
        public abstract void OnExecuting(ActionContext actionContext);
        /// <summary>
        /// 执行后
        /// </summary>
        /// <param name="actionContext"></param>
        public abstract void Executed(ActionContext actionContext);
    }
}
