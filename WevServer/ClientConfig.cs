using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WebServer
{
    public class ClientConfig
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public string ClientDir { get; set; }

        public static List<ClientConfig> Init()
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ClientConfig.json");
            using (var reader =new  StreamReader(File.OpenRead(filePath), Encoding.UTF8))
            {
                return JsonConvert.DeserializeObject<List<ClientConfig>>(reader.ReadToEnd());
            }
        }
    }
}
