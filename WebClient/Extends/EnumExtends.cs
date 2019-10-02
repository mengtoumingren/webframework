using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Attributes;

namespace WebClient.Extends
{
    public static class EnumExtends
    {
        public static string Remark(this Enum value)
        {
            var attr = (RemarkAttribute)value.GetType().GetMember(value.ToString())[0].GetCustomAttributes(typeof(RemarkAttribute), false)?.FirstOrDefault();
            return attr == null ? value.ToString() : attr.Value;
        }
    }
}
