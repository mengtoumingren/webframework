using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHost.WebClient.Mvc;

namespace WebHost.WebClient.Mvc
{
    public class ControllerFactory
    {
        public virtual BaseController Create(Type controllerType)
        {
            try
            {
                return (BaseController)Activator.CreateInstance(controllerType);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
