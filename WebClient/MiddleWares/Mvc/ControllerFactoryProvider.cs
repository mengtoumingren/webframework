using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Mvc;

namespace WebClient.MiddleWares.Mvc
{
    public class ControllerFactoryProvider
    {
        private static ControllerFactory controllerFactory;

        /// <summary>
        /// 获取控制器工厂
        /// </summary>
        /// <returns></returns>
        public static ControllerFactory GetControllerFactory()
        {
            return controllerFactory;
        }
        /// <summary>
        /// 设置自定义控制器工厂
        /// </summary>
        /// <param name="factory"></param>
        public static void SetControllerFactory(ControllerFactory factory)
        {
            if (controllerFactory != null)
                controllerFactory = factory;
        }
    }
}
