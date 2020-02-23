using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHost.WebClient
{
    public class HttpPostedFile
    {
        public string FileName { get; }
        public string ContentType { get; }
        public int ContentLength { get; }
        public Stream InputStream { get; }
    }
}
