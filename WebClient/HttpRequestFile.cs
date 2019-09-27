using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class HttpRequestFile
    {
        public string Name { get; internal set; }
        public string Filename { get; internal set; }
        public string ContentType { get; internal set; }
        public Stream FileStream { get; internal set; }
    }
}
