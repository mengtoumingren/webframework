using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Attributes
{
    public class RemarkAttribute:Attribute
    {
        public RemarkAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
